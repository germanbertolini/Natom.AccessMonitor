using Microsoft.Extensions.DependencyInjection;
using Natom.AccessMonitor.Services.Logger.PackageConfig;
using Natom.AccessMonitor.Services.Logger.HostedServices;
using Natom.AccessMonitor.Services.Logger.Services;
using Natom.AccessMonitor.Services.Logger.Entities;

namespace Natom.AccessMonitor.Extensions
{
    public static class StartupExtensions
    {
        private static bool _hostedServiceAdded = false;

        public static IServiceCollection AddLoggerService(this IServiceCollection service, string systemName, int insertEachMS = 30000, int bulkInsertSize = 10000)
        {
            service.AddSingleton<DiscordService>();
            service.AddSingleton<LoggerService>();
            service.AddSingleton(new LoggerServiceConfig
            {
                SystemName = systemName,
                InsertEachMS = insertEachMS,
                BulkInsertSize = bulkInsertSize
            });

            service.AddScoped<Transaction>();

            if (!_hostedServiceAdded)
            {
                service.AddHostedService<LoggerTimedHostedService>();
                _hostedServiceAdded = true;
            }
            return service;
        }
    }
}
