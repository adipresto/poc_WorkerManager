using MainAplikasi.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkerManager.Core.Contexts;
using WorkerManager.Core.Interfaces;

namespace MainAplikasi
{
    public class ContextWorkerService : IScopedContextWorkerService
    {
        private readonly ILogger<ContextWorkerService> _logger;

        public ContextWorkerService(ILogger<ContextWorkerService> logger)
        {
            _logger = logger;
        }

        public async Task DoWorkAsync(WorkerContext context, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker {WorkerId} started at {StartTime}",
                context.WorkerId, context.StartTime);

            var name = context.GetParameter<string>("name", "Mahmud");
            var priority = context.GetParameter<int>("priority", 1);
            var config = context.GetParameter<WorkerConfig>("config");

            _logger.LogInformation("Processing work for {Name} with priority {Priority}", name, priority);

            if (config != null)
            {
                _logger.LogInformation("Using config: BatchSize={BatchSize}, Timeout={Timeout}",
                    config.BatchSize, config.Timeout);
            }

            await Task.Delay(config?.Timeout ?? TimeSpan.FromSeconds(2), cancellationToken);
        }
    }

}
