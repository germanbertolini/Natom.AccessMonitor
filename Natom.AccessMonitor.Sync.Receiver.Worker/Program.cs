using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Natom.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Receiver.Worker
{
    public class Program
    {
        private static IConfiguration Configuration { get; set; }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            var host = Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddConfigurationService(refreshTimeMS: 30000)
                    .AddLoggerService(systemName: "Sync.Receiver.Worker", insertEachMS: 30000, bulkInsertSize: 10000)
                    .AddMQProducerService()
                    .AddMQConsumerService();

                    services.AddHostedService<MQWorker>();
                    services.AddHostedService<MovementsProcessorWorker>();
                });
            return host;
        }
    }
}
