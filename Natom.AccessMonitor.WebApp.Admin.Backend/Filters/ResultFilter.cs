using Microsoft.AspNetCore.Mvc.Filters;
using Natom.AccessMonitor.Services.Auth.Entities;
using Natom.AccessMonitor.Services.Logger.Entities;
using Natom.AccessMonitor.Services.Logger.Services;
using System;

namespace Natom.AccessMonitor.WebApp.Admin.Backend.Filters
{
    public class ResultFilter : IResultFilter
    {
        private readonly LoggerService _loggerService;
        private readonly Transaction _transaction;
        private readonly AccessToken _accessToken;

        public ResultFilter(IServiceProvider serviceProvider)
        {
            _loggerService = (LoggerService)serviceProvider.GetService(typeof(LoggerService));
            _transaction = (Transaction)serviceProvider.GetService(typeof(Transaction));
            _accessToken = (AccessToken)serviceProvider.GetService(typeof(AccessToken));
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {

        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            _loggerService.LogInfo(_transaction.TraceTransactionId, "Fin transacción");
        }
    }
}
