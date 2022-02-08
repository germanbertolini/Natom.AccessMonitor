using Microsoft.Extensions.DependencyInjection;
using Natom.AccessMonitor.Services.Mailer.Services;

namespace Natom.AccessMonitor.Extensions
{
    public static class StartupExtensions
    {
        private static bool _hostedServiceAdded = false;

        public static IServiceCollection AddMailService(this IServiceCollection service)
        {
            service.AddSingleton<MailService>();

            return service;
        }
    }
}