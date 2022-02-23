using Microsoft.AspNetCore.Mvc;
using Natom.AccessMonitor.Sync.Entities.DTO;
using Natom.AccessMonitor.Sync.Receiver.Repositories;
using System;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Receiver.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class EchoController : BaseController
    {
        public EchoController(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        [HttpGet]
        [ActionName("")]
        public IActionResult Get()
        {
            return Ok(DateTimeOffset.Now);
        }

        [HttpGet]
        [ActionName("HealthCheckForTransmitter")]
        public IActionResult GetHealthCheckForTransmitter()
        {
            if (!string.IsNullOrEmpty(_accessToken.SyncInstanceId))
            {
                Task.Run(async () =>
                {
                    var syncRepository = new SynchronizerRepository(_configurationService);
                    await syncRepository.RegisterConnectionAsync(_accessToken.SyncInstanceId);
                });
            }
            return Ok(new TransmitterResponseDto { Success = true });
        }
    }
}
