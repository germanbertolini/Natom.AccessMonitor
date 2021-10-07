using Microsoft.AspNetCore.Mvc;
using Natom.AccessMonitor.Services.Auth.Services;
using Natom.AccessMonitor.Services.Cache.Services;
using Natom.AccessMonitor.Sync.Receiver.Entities.DTO;
using Natom.AccessMonitor.Sync.Receiver.Entities.MQ;
using Natom.AccessMonitor.Sync.Receiver.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IActionResult> PostUploadAsync([FromBody]string content)
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
                    Message = content
                };
                await _mqService.PublishAsync(message, queue);
                return Ok(new TransmitterResponseDto { Success = true });
            }
            catch (Exception ex)
            {
                return Ok(new TransmitterResponseDto { Success = false, Error = ex.Message });
            }
        }
    }
}
