using Microsoft.AspNetCore.Mvc;
using Natom.AccessMonitor.Sync.Entities.DTO;
using System;
using StackExchange.Redis;
using Natom.Extensions.Cache.Services;
using Natom.Extensions.Auth.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using Natom.Extensions.Auth.Entities;
using Natom.Extensions.Logger.Entities;
using Natom.Extensions.Logger.Services;
using Natom.AccessMonitor.Sync.Receiver.Repositories;

namespace Natom.AccessMonitor.Sync.Receiver.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ActivationController : BaseController
    {
        private readonly AuthService _authService;
        private readonly CacheService _cacheService;
        private readonly DiscordService _discordService;


        public ActivationController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _authService = (AuthService)serviceProvider.GetService(typeof(AuthService));
            _cacheService = (CacheService)serviceProvider.GetService(typeof(CacheService));
            _discordService = (DiscordService)serviceProvider.GetService(typeof(DiscordService));
        }

        [HttpPost]
        [ActionName("Start")]
        public async Task<IActionResult> PostStartAsync(StartActivationHandshakeDto instanceInfo)
        {
            try
            {
                var permissions = new List<string>()
                {
                    $"{typeof(ActivationController).Name}.*"
                };
                var tokenDurationMinutes = 2 * 60; //TOKEN TEMPORAL VALIDO POR 2 HORAS
                var authToken = await _authService.CreateTokenForSynchronizerAsync(instanceId: instanceInfo.InstanceId, userName: instanceInfo.InstallationAlias, clientId: null, clientName: instanceInfo.ClientName, permissions, tokenDurationMinutes);

                var syncRepository = new SynchronizerRepository(_configurationService);
                await syncRepository.InsertAsPendingAsync(new Entities.Models.Synchronizer
                {
                    InstanceId = instanceInfo.InstanceId,
                    InstallationAlias = instanceInfo.InstallationAlias,
                    InstallerName = instanceInfo.InstallerName,
                    CurrentSyncToServerMinutes = instanceInfo.CurrentSyncToServerMinutes,
                    CurrentSyncFromDevicesMinutes = instanceInfo.CurrentSyncFromDevicesMinutes,
                    InstalledAt = DateTime.Now
                });

                await AddToActivationQueueAsync(instanceInfo);

                return Ok(new TransmitterResponseDto<dynamic>
                {
                    Success = true,
                    Data = new { accessToken = authToken.ToJwtEncoded() }
                });
            }
            catch (Exception ex)
            {
                return Ok(new TransmitterResponseDto{ Success = false, Error = ex.Message });
            }
        }


        [HttpGet]
        [ActionName("Status")]
        public async Task<IActionResult> GetStatusAsync()
        {
            try
            {
                var synchronizer = await GetSynchronizerStatusAsync(_accessToken.SyncInstanceId);
                var activated = synchronizer?.ActivatedAt != null;

                return Ok(new TransmitterResponseDto<dynamic>
                {
                    Success = true,
                    Data = new { activated }
                });
            }
            catch (Exception ex)
            {
                return Ok(new TransmitterResponseDto { Success = false, Error = ex.Message });
            }
        }


        [HttpPost]
        [ActionName("Confirm")]
        public async Task<IActionResult> PostConfirmAsync()
        {
            try
            {
                var synchronizer = await GetSynchronizerStatusAsync(_accessToken.SyncInstanceId);
                if (synchronizer?.ActivatedAt == null)
                    throw new Exception("El sincronizador no fue activado.");

                int clientId = synchronizer.ActivatedToClientId.Value;

                var syncRepository = new SynchronizerRepository(_configurationService);
                await syncRepository.MarkAsCompletedAsync(_accessToken.SyncInstanceId, clientId);

                await RemoveFromActivationQueueAsync(_accessToken.SyncInstanceId);

                var permissions = new List<string>()
                {
                    $"{typeof(SyncController).Name}.*"
                };
                long tokenDurationMinutes = 60 * 24 * 30 * 12 * 100; //100 AÑOS
                var definitiveAccessToken = await _authService.CreateTokenForSynchronizerAsync(instanceId: _accessToken.SyncInstanceId, userName: _accessToken.UserFullName, clientId, clientName: _accessToken.ClientFullName, permissions, tokenDurationMinutes);

                //DESTRUIREMOS EL TOKEN TEMPORAL DENTRO DE 10 SEGUNDOS YA QUE PUEDE ESTAR CONSULTANDO EL ESTADO DEL SERVICIO
                //var accessTokenScope = _accessToken.Scope;
                //var accessTokenKey = _accessToken.Key;
                //_ = Task.Run(async () =>
                //{
                //    await Task.Delay(10000);
                //    _authService.DestroyTokenAsync(accessTokenScope, accessTokenKey);
                //});

                //NOTIFICAMOS VIA DISCORD LA NUEVA ACTIVACIÓN
                _ = Task.Run(() => _discordService.LogInfoAsync("Nuevo sincronizador activado.", _transaction.TraceTransactionId, synchronizer));


                return Ok(new TransmitterResponseDto<dynamic>
                {
                    Success = true,
                    Data = new { activated = true, accessToken = definitiveAccessToken.ToJwtEncoded() }
                });
            }
            catch (Exception ex)
            {
                return Ok(new TransmitterResponseDto { Success = false, Error = ex.Message });
            }
        }

        
        private async Task AddToActivationQueueAsync(StartActivationHandshakeDto newInstance)
        {
            var synchronizer = new PendingToActivateDto
            {
                InstanceId = newInstance.InstanceId,
                InstallerName = newInstance.InstallerName,
                InstallationAlias = newInstance.InstallationAlias,
                ClientCUIT = newInstance.ClientCUIT,
                ClientName = newInstance.ClientName,
                DateTime = DateTime.Now,
                ActivatedAt = null
            };
            await _cacheService.SetValueAsync($"Sync.Receiver.Activation.Queue.{synchronizer.InstanceId}", JsonConvert.SerializeObject(synchronizer), TimeSpan.FromHours(2));
        }

        private async Task<PendingToActivateDto> GetSynchronizerStatusAsync(string instanceId)
        {
            PendingToActivateDto synchronizer = null;
            var data = await _cacheService.GetValueAsync($"Sync.Receiver.Activation.Queue.{instanceId}");
            if (!string.IsNullOrEmpty(data))
            {
                synchronizer = JsonConvert.DeserializeObject<PendingToActivateDto>(data);
            }
            return synchronizer;
        }

        private async Task RemoveFromActivationQueueAsync(string instanceId)
        {
            await _cacheService.RemoveAsync($"Sync.Receiver.Activation.Queue.{instanceId}");
        }
    }
}
