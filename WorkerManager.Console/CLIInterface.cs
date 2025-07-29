using WorkerManager.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WorkerManager.Console
{
    public class CLIInterface : BackgroundService
    {
        private readonly IWorkerCommandProcessor _commandProcessor;
        private readonly ILogger<CLIInterface> _logger;

        public CLIInterface(IWorkerCommandProcessor commandProcessor, ILogger<CLIInterface> logger)
        {
            _commandProcessor = commandProcessor;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker Management Console Interface started");
            _logger.LogInformation("Type 'H' for help, 'Q' to quit");

            await Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        System.Console.Write("Worker> ");
                        var input = System.Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(input))
                            continue;

                        if (input.Trim().Equals("Q", StringComparison.OrdinalIgnoreCase) ||
                            input.Trim().Equals("QUIT", StringComparison.OrdinalIgnoreCase))
                        {
                            _logger.LogInformation("Shutting down...");
                            break;
                        }

                        var result = await _commandProcessor.ProcessCommandAsync(input);
                        System.Console.WriteLine(result);
                        System.Console.WriteLine();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in console interface");
                        System.Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            }, stoppingToken);
        }
    }

    public static class ConsoleExtensions
    {
        public static IServiceCollection AddWorkerConsoleInterface(this IServiceCollection services)
        {
            services.AddHostedService<CLIInterface>();
            return services;
        }
    }

}
