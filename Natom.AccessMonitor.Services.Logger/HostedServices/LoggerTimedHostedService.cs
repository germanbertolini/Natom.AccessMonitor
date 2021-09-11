using Microsoft.Extensions.Hosting;
using Natom.AccessMonitor.Services.Configuration.Services;
using Natom.AccessMonitor.Services.Logger.PackageConfig;
using Natom.AccessMonitor.Services.Logger.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Services.Logger.HostedServices
{
    public class LoggerTimedHostedService : IHostedService, IDisposable
    {
        private readonly LoggerService _loggerService;
        private readonly LoggerServiceConfig _config;

        private int executionCount = 0;
        private Timer _timer;

        public LoggerTimedHostedService(IServiceProvider serviceProvider)
        {
            _loggerService = (LoggerService)serviceProvider.GetService(typeof(LoggerService));
            _config = (LoggerServiceConfig)serviceProvider.GetService(typeof(LoggerServiceConfig));
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, _config.InsertEachMS, _config.InsertEachMS);

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var count = Interlocked.Increment(ref executionCount);
            try
            {
                if (_loggerService != null)
                    _loggerService.BulkInsertAsync().Wait();
            }
            catch (Exception ex)
            {

            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
