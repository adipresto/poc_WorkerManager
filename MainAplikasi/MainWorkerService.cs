using Microsoft.Extensions.Logging;
using WorkerManager.Core.Interfaces;

namespace MainAplikasi
{
    public class MainWorkerService : IScopedWorkerService
    {
        private readonly ILogger<MainWorkerService> _logger;

        public MainWorkerService(ILogger<MainWorkerService> logger)
        {
            _logger = logger;
        }

        public async Task DoWorkAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Console worker doing work...");
            await Task.Delay(3000, cancellationToken);
        }
    }
}
