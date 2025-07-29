using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkerManager.Core.Interfaces;

namespace MainAplikasi
{
    public class TypedWorkerService : IScopedWorkerService<string>
    {
        private readonly ILogger<TypedWorkerService> _logger;

        public TypedWorkerService(ILogger<TypedWorkerService> logger)
        {
            _logger = logger;
        }

        public async Task DoWorkAsync(string parameter, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing string parameter: {Parameter}", parameter);
            await Task.Delay(2000, cancellationToken);
        }
    }
}
