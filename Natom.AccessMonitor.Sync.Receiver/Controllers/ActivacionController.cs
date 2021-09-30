using Microsoft.AspNetCore.Mvc;
using Natom.AccessMonitor.Sync.Receiver.Entities.DTO;
using System;
using StackExchange.Redis;
using Natom.AccessMonitor.Services.Cache.Services;

namespace Natom.AccessMonitor.Sync.Receiver.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ActivacionController : BaseController
    {
        private readonly CacheService _cacheService;

        public ActivacionController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _cacheService = (CacheService)serviceProvider.GetService(typeof(CacheService));
        }

        [HttpPost]
        [ActionName("StartHandshake")]
        public IActionResult PostStartHandshake(StartActivationHandshakeDto instanceInfo)
        {
            try
            {
                var secretKey = $"{Guid.NewGuid():N}";
                _cacheService.SetValueAsync(instanceInfo.InstanceId, secretKey, TimeSpan.FromMinutes(60));

                return Ok(new TransmitterResponseDto<dynamic>
                {
                    Success = true,
                    Data = new
                    {
                        secretKey
                    }
                });
            }
            catch (Exception ex)
            {
                return Ok(new TransmitterResponseDto{ Success = false, Error = ex.Message });
            }
        }
    }
}
