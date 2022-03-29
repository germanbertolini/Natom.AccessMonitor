using Microsoft.AspNetCore.Mvc;
using Natom.Extensions.Auth.Entities;
using Natom.Extensions.Configuration.Services;
using Natom.Extensions.Logger.Entities;
using Natom.Extensions.Logger.Services;
using System;

namespace Natom.AccessMonitor.Sync.Receiver.Controllers
{
    public class BaseController : ControllerBase
    {
        protected readonly ConfigurationService _configurationService;
        protected readonly LoggerService _loggerService;
        protected readonly Transaction _transaction;
        protected readonly AccessToken _accessToken;

        public BaseController(IServiceProvider serviceProvider)
        {
            _configurationService = (ConfigurationService)serviceProvider.GetService(typeof(ConfigurationService));
            _loggerService = (LoggerService)serviceProvider.GetService(typeof(LoggerService));

            _accessToken = (AccessToken)serviceProvider.GetService(typeof(AccessToken));
            _transaction = (Transaction)serviceProvider.GetService(typeof(Transaction));
        }
    }
}
