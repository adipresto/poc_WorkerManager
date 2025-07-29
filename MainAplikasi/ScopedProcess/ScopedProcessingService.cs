using MainAplikasi.ScopedProcess.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainAplikasi.ScopedProcess
{
    public class ScopedProcessingService : IScopedProcessingService
    {
        private int executionCount = 0;
        private readonly ILogger _logger;

        public ScopedProcessingService(ILogger<ScopedProcessingService> logger)
        {
            _logger = logger;
        }

        public async Task DoWork(int workerId, CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                executionCount++;

                _logger.LogInformation(
                    "WorkerID: {WorkerID} || Scoped Processing Service is working. Count: {Count}", workerId, executionCount);

                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
