using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Natom.AccessMonitor.Services.Auth.Entities;
using Natom.AccessMonitor.Services.Configuration.Services;
using Natom.AccessMonitor.Services.Logger.Entities;
using Natom.AccessMonitor.Services.Logger.Services;
using Natom.AccessMonitor.Services.Mailer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.WebApp.Clientes.Backend.Controllers
{
    public class BaseController : ControllerBase
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly ConfigurationService _configurationService;
        protected readonly LoggerService _loggerService;
        protected readonly Transaction _transaction;
        protected readonly MailService _mailService;
        protected readonly AccessToken _accessToken;

        public BaseController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _configurationService = (ConfigurationService)serviceProvider.GetService(typeof(ConfigurationService));
            _loggerService = (LoggerService)serviceProvider.GetService(typeof(LoggerService));

            _accessToken = (AccessToken)serviceProvider.GetService(typeof(AccessToken));
            _transaction = (Transaction)serviceProvider.GetService(typeof(Transaction));
            _mailService = (MailService)serviceProvider.GetService(typeof(MailService));
        }

        protected string GetAuthorizationFromHeader()
        {
            string authorization = null;
            StringValues stringValues;
            if (Request.Headers.TryGetValue("Authorization", out stringValues))
                authorization = stringValues.FirstOrDefault();
            return authorization;
        }
    }
}
