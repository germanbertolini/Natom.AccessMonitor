using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Natom.AccessMonitor.Sync.Receiver.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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
            return Ok(new TransmitterDTO { Success = true });
        }
    }
}
