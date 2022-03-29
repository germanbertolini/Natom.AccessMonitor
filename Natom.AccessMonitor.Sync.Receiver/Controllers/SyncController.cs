using Microsoft.AspNetCore.Mvc;
using Natom.Extensions.Auth.Services;
using Natom.Extensions.Cache.Services;
using Natom.Extensions.MQ.Entities;
using Natom.Extensions.MQ.Services;
using Natom.AccessMonitor.Sync.Entities.DTO;
using Natom.AccessMonitor.Sync.Receiver.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Natom.AccessMonitor.Sync.Receiver.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class SyncController : BaseController
    {
        private readonly AuthService _authService;
        private readonly CacheService _cacheService;
        private readonly MQService _mqService;

        public SyncController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _authService = (AuthService)serviceProvider.GetService(typeof(AuthService));
            _cacheService = (CacheService)serviceProvider.GetService(typeof(CacheService));
            _mqService = (MQService)serviceProvider.GetService(typeof(MQService));
        }

        [HttpPost]
        [ActionName("Upload")]
        public async Task<IActionResult> PostUploadAsync([FromBody]string content, [FromQuery]string rules)
        {
            try
            {
                var queue = new QueueMQ
                {
                    Exchange = "ACCESSMONITOR",
                    QueueName = "ACCESSMONITOR_SIGNS",
                    RoutingKey = "accessmonitor/signs"
                };
                var message = new MessageMQ
                {
                    Topic = "DataBlockToBulkInsert",
                    CreationDateTime = DateTime.Now,
                    ProducerInfo = new ProducerInfoMQ {
                        ClientId = _accessToken.ClientId.Value,
                        SyncInstanceId = _accessToken.SyncInstanceId
                    },
                    Message = content
                };

                int? currentSyncToServerMinutes = null;
                int? currentSyncFromDevicesMinutes = null;
                object configObj = null;
                if (!string.IsNullOrEmpty(rules))
                {
                    var config = JsonConvert.DeserializeObject<dynamic>(HttpUtility.UrlDecode(Uri.UnescapeDataString(rules)));
                    currentSyncToServerMinutes = Convert.ToInt32(config.stsm);
                    currentSyncFromDevicesMinutes = Convert.ToInt32(config.sfdm);
                }

                if (!string.IsNullOrEmpty(_accessToken.SyncInstanceId))
                {
                    var syncRepository = new SynchronizerRepository(_configurationService);
                    try
                    {
                        var config = await syncRepository.RegisterSyncAndGetConfigAsync(_accessToken.SyncInstanceId, currentSyncToServerMinutes, currentSyncFromDevicesMinutes);
                        if (config != null && (config.NewSyncFromDevicesMinutes.HasValue || config.NewSyncToServerMinutes.HasValue))
                            configObj = new
                            {
                                NewSyncFromDevicesMinutes = config.NewSyncFromDevicesMinutes,
                                NewSyncToServerMinutes = config.NewSyncToServerMinutes
                            };
                    }
                    catch (SqlException) { }
                    catch (Exception ex) { throw ex; }
                }

                await _mqService.PublishAsync(message, queue);
                return Ok(new TransmitterResponseDto { Success = true, Config = configObj });
            }
            catch (Exception ex)
            {
                return Ok(new TransmitterResponseDto { Success = false, Error = ex.Message });
            }
        }
    }
}
