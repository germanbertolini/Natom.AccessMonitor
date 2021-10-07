using Microsoft.AspNetCore.Mvc;
using Natom.AccessMonitor.Sync.Entities.DTO;
using System;

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
            return Ok(new TransmitterResponseDto { Success = true });
        }
    }
}
