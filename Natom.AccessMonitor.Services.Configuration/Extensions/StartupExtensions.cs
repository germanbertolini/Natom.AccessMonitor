using Microsoft.Extensions.DependencyInjection;
using Natom.AccessMonitor.Services.Configuration.HostedServices;
using Natom.AccessMonitor.Services.Configuration.Services;

namespace Natom.AccessMonitor.Extensions
{
    public static class StartupExtensions
    {
        private static bool _hostedServiceAdded = false;

        public static IServiceCollection AddConfigurationService(this IServiceCollection service)
        {
            service.AddSingleton<ConfigurationService>();
            if (!_hostedServiceAdded)
            {
                service.AddHostedService<ConfigTimedHostedService>();
                _hostedServiceAdded = true;
            }
            return service;
        }
    }
}
