using Microsoft.AspNetCore.Mvc;
using Natom.AccessMonitor.Services.Configuration.Services;
using Natom.AccessMonitor.Services.Logger.Services;
using System;

namespace Natom.AccessMonitor.Sync.Receiver.Controllers
{
    public class BaseController : ControllerBase
    {
        protected readonly ConfigurationService _configurationService;
        protected readonly LoggerService _loggerService;

        public BaseController(IServiceProvider serviceProvider)
        {
            _configurationService = (ConfigurationService)serviceProvider.GetService(typeof(ConfigurationService));
            _loggerService = (LoggerService)serviceProvider.GetService(typeof(LoggerService));
        }
    }
}
