using Microsoft.Extensions.Hosting;
using Natom.AccessMonitor.Services.Configuration.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Services.Configuration.HostedServices
{
    public class ConfigTimedHostedService : IHostedService, IDisposable
    {
        private readonly ConfigurationService _configService;

        private int executionCount = 0;
        private Timer _timer;

        public ConfigTimedHostedService(IServiceProvider serviceProvider)
        {
            _configService = (ConfigurationService)serviceProvider.GetService(typeof(ConfigurationService));
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, 0, 30000);

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var count = Interlocked.Increment(ref executionCount);
            try
            {
                if (_configService != null)
                    _configService.RefreshAsync().Wait();
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
