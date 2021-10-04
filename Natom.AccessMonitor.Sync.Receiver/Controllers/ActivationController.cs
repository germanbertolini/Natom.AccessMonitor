using Microsoft.AspNetCore.Mvc;
using Natom.AccessMonitor.Sync.Receiver.Entities.DTO;
using System;
using StackExchange.Redis;
using Natom.AccessMonitor.Services.Cache.Services;
using Natom.AccessMonitor.Services.Auth.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using Natom.AccessMonitor.Services.Auth.Entities;
using Natom.AccessMonitor.Services.Logger.Entities;

namespace Natom.AccessMonitor.Sync.Receiver.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ActivationController : BaseController
    {
        private readonly AuthService _authService;
        private readonly CacheService _cacheService;
        
        public ActivationController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _authService = (AuthService)serviceProvider.GetService(typeof(AuthService));
            _cacheService = (CacheService)serviceProvider.GetService(typeof(CacheService));
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
                var activated = await CheckForActivationAsync(_accessToken.SyncInstanceId);

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
                var activated = await CheckForActivationAsync(_accessToken.SyncInstanceId);
                if (!activated)
                    throw new Exception("El sincronizador no fue activado.");

                //await UpdateFinalActivationTimeAsync(_accessToken.SyncInstanceId);

                await RemoveFromActivationQueueAsync(_accessToken.SyncInstanceId);

                var permissions = new List<string>()
                {
                    $"{typeof(SyncController).Name}.*"
                };
                long tokenDurationMinutes = 60 * 24 * 30 * 12 * 30; //30 AÑOS
                var definitiveAccessToken = await _authService.CreateTokenForSynchronizerAsync(instanceId: _accessToken.SyncInstanceId, userName: _accessToken.UserFullName, clientId: null, clientName: _accessToken.ClientFullName, permissions, tokenDurationMinutes);

                return Ok(new TransmitterResponseDto<dynamic>
                {
                    Success = true,
                    Data = new { activated, accessToken = definitiveAccessToken.ToJwtEncoded() }
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

        private async Task<bool> CheckForActivationAsync(string instanceId)
        {
            bool activated = false;
            var data = await _cacheService.GetValueAsync($"Sync.Receiver.Activation.Queue.{instanceId}");
            if (!string.IsNullOrEmpty(data))
            {
                var synchronizer = JsonConvert.DeserializeObject<PendingToActivateDto>(data);
                activated = synchronizer.ActivatedAt.HasValue;
            }
            return activated;
        }

        private async Task RemoveFromActivationQueueAsync(string instanceId)
        {
            await _cacheService.RemoveAsync($"Sync.Receiver.Activation.Queue.{instanceId}");
        }
    }
}
