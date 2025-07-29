using MainAplikasi.ScopedProcess.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainAplikasi.ScopedProcess
{
    public class ConsumeScopedHostedService : BackgroundService, IWorkManager
    {
        private readonly ILogger<ConsumeScopedHostedService> _logger;
        private readonly ConcurrentDictionary<int, CancellationTokenSource> _workers;
        private int _nextWorkerId = 0;

        public ConsumeScopedHostedService(IServiceProvider services,
            ILogger<ConsumeScopedHostedService> logger)
        {
            Services = services;
            _logger = logger;
        }

        public async Task AddWorker()
        {
            var workerId = Interlocked.Increment(ref _nextWorkerId);
            var cts = new CancellationTokenSource();

            if (_workers.TryAdd(workerId, cts))
            {
                _ = Task.Run(() => DoWork(workerId, cts.Token));
                _logger.LogInformation("Added worker {WorkerId}", workerId);
            }
        }

        public async Task RemoveWorker()
        {
            if (_workers.Count > 1)
            {
                var worker = _workers.First();
                if (_workers.TryRemove(worker.Key, out var cts))
                {
                    cts.Cancel();
                    _logger.LogInformation("Removed worker {WorkerId}", worker.Key);
                }
            }
        }

        public int GetActiveWorkerCount() => _workers.Count;

        public IServiceProvider Services { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var workerId = Interlocked.Increment(ref _nextWorkerId);

            _logger.LogInformation(
                "Consume Scoped Service Hosted Service running.");

            await DoWork(workerId, stoppingToken);
        }

        private async Task DoWork(int workerId, CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Consume Scoped Service Hosted Service is working.");

            using (var scope = Services.CreateScope())
            {
                var scopedProcessingService =
                    scope.ServiceProvider
                        .GetRequiredService<IScopedProcessingService>();

                await scopedProcessingService.DoWork(workerId, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Consume Scoped Service Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
