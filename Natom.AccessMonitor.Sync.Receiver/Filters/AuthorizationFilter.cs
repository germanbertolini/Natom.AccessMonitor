using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Natom.AccessMonitor.Common.Exceptions;
using Natom.AccessMonitor.Common.Helpers;
using Natom.AccessMonitor.Services.Logger.Entities;
using Natom.AccessMonitor.Services.Logger.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Receiver.Filters
{
    public class AuthorizationFilter : ActionFilterAttribute, IAsyncAuthorizationFilter
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _accessor;
        private readonly LoggerService _loggerService;

        private string _controller = null;
        private string _action = null;
        private string _urlRequested = null;
        private string _actionRequested = null;
        private Transaction _transaction = null;


        public AuthorizationFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _loggerService = (LoggerService)serviceProvider.GetService(typeof(LoggerService));

            _accessor = (IHttpContextAccessor)serviceProvider.GetService(typeof(IHttpContextAccessor));
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            object accessToken = null;

            this.Init(context);
            try
            {
                //VALIDACIONES DE SEGURIDAD
                if (!_controller.Equals("security") && !_action.Equals("getecho"))
                {
                    var headerValuesForAuthorization = context.HttpContext.Request.Cookies["Authorization"];
                    if (headerValuesForAuthorization == null || headerValuesForAuthorization.Count() == 0)
                        throw new HandledException("Se debe enviar el 'Authorization'.");

                    if (!headerValuesForAuthorization.ToString().StartsWith("Bearer"))
                        throw new HandledException("'Authorization' inválido.");

                    string authorization = headerValuesForAuthorization.ToString().Replace("Bearer ", "");

                    //accessToken = await _authService.DecodeAndValidateTokenAsync(authorization, _transactionService.Language);
                    //_permissionService.ValidatePermission(accessToken.Roles, _actionRequested);
                    
                    
                    _loggerService.LogInfo(_transaction.TraceTransactionId, "Token autorizado");
                }
            }
            catch (HandledException ex)
            {
                _loggerService.LogBounce(_transaction?.TraceTransactionId, ex.Message, accessToken);

                context.HttpContext.Response.StatusCode = 403;
                context.Result = new ContentResult()
                {
                    Content = ex.Message
                };
            }
            catch (Exception ex)
            {
                _loggerService.LogException(_transaction?.TraceTransactionId, ex);

                context.HttpContext.Response.StatusCode = 500;
                context.Result = new ContentResult()
                {
                    Content = "Se ha producido un error interno al autenticar."
                };
            }
        }

        private void Init(AuthorizationFilterContext context)
        {
            var actionDescriptor = ((ControllerActionDescriptor)context.ActionDescriptor);
            var ip = GetRealIP(context);
            var agent = context.HttpContext.Request.Headers["User-Agent"].ToString();
            var os = RequestHelper.GetOSFromUserAgent(agent);
            if (os != null && os.Length > 30) os = os.Substring(0, 30);
            var appVersion = GetAppVersion(context);
            var lang = GetLanguage(context);

            _controller = actionDescriptor.RouteValues["controller"].ToLower();
            _action = actionDescriptor.RouteValues["action"].ToLower();
            _urlRequested = String.Format("[{0}] {1} {2}", context.HttpContext.Request.Scheme.ToUpper(), context.HttpContext.Request.Method, context.HttpContext.Request.Path.Value);
            _actionRequested = actionDescriptor.ControllerTypeInfo.FullName + "." + actionDescriptor.RouteValues["action"];
            
            var scope = typeof(Startup).Assembly.GetName().Name;
            var hostname = Dns.GetHostName();
            var port = context.HttpContext.Connection.LocalPort;

            int? userId = null;

            _transaction = _loggerService.CreateTransaction(scope, lang, ip, _urlRequested, _actionRequested, userId, os, appVersion, hostname, port);
        }

        private string GetRealIP(AuthorizationFilterContext context)
        {
            var forwardedFor = context.HttpContext.Request.Headers["HTTP_X_FORWARDED_FOR"];

            var userIpAddress = forwardedFor.Count() == 0 || String.IsNullOrWhiteSpace(forwardedFor.ToString())
                                    ? _accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString()
                                    : forwardedFor.ToString().Split(',').Select(s => s.Trim()).FirstOrDefault();

            return userIpAddress;
        }

        private string GetAppVersion(AuthorizationFilterContext context)
        {
            var headerValue = context.HttpContext.Request.Headers["APP_VERSION"];
            var appVersion = headerValue.Count() == 0 || String.IsNullOrWhiteSpace(headerValue.ToString())
                                    ? null
                                    : headerValue.ToString();
            return appVersion;
        }

        private string GetLanguage(AuthorizationFilterContext context)
        {
            var headerValue = context.HttpContext.Request.Headers["LANG"];
            var lang = headerValue.Count() == 0 || String.IsNullOrWhiteSpace(headerValue.ToString())
                                    ? null
                                    : headerValue.ToString();
            return lang;
        }
    }
}
