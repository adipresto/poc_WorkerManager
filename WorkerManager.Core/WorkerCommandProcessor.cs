using WorkerManager.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace WorkerManager.Core
{
    // Command Processor
    public class WorkerCommandProcessor : IWorkerCommandProcessor
    {
        private readonly IWorkManager _workManager;
        private readonly ILogger<WorkerCommandProcessor> _logger;
        private readonly Dictionary<string, Func<Task<string>>> _commands;

        public WorkerCommandProcessor(IWorkManager workManager, ILogger<WorkerCommandProcessor> logger)
        {
            _workManager = workManager;
            _logger = logger;

            _commands = new Dictionary<string, Func<Task<string>>>(StringComparer.OrdinalIgnoreCase)
            {
                ["A"] = AddWorkerAsync,
                ["ADD"] = AddWorkerAsync,
                ["D"] = RemoveWorkerAsync,
                ["DELETE"] = RemoveWorkerAsync,
                ["REMOVE"] = RemoveWorkerAsync,
                ["L"] = ListWorkersAsync,
                ["LIST"] = ListWorkersAsync,
                ["STATUS"] = ListWorkersAsync,
                ["S"] = StopAllWorkersAsync,
                ["STOP"] = StopAllWorkersAsync,
                ["H"] = ShowHelpAsync,
                ["HELP"] = ShowHelpAsync
            };
        }

        public async Task<string> ProcessCommandAsync(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                return "Please provide a command. Type 'H' for help.";

            try
            {
                var cmd = command.Trim().ToUpper();

                if (_commands.TryGetValue(cmd, out var commandFunc))
                {
                    return await commandFunc();
                }

                return $"Unknown command: '{command}'. Type 'H' for available commands.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing command: {Command}", command);
                return $"Error processing command: {ex.Message}";
            }
        }

        public Dictionary<string, string> GetAvailableCommands()
        {
            return new Dictionary<string, string>
            {
                ["A/ADD"] = "Add a new worker",
                ["D/DELETE/REMOVE"] = "Remove a worker",
                ["L/LIST/STATUS"] = "List active workers",
                ["S/STOP"] = "Stop all workers",
                ["H/HELP"] = "Show this help"
            };
        }

        private async Task<string> AddWorkerAsync()
        {
            await _workManager.AddWorkerAsync(1);
            var activeCount = _workManager.GetActiveWorkerCount();
            return $"Added 1 worker. Active workers: {activeCount}";
        }

        private async Task<string> RemoveWorkerAsync()
        {
            var removed = await _workManager.RemoveWorkerAsync(1);
            var activeCount = _workManager.GetActiveWorkerCount();

            if (removed)
                return $"Removed 1 worker. Active workers: {activeCount}";
            else
                return $"Could not remove worker. Active workers: {activeCount}";
        }

        private async Task<string> ListWorkersAsync()
        {
            var activeCount = _workManager.GetActiveWorkerCount();
            var workerDetails = _workManager.GetWorkerDetails();

            if (!workerDetails.Any())
                return "No active workers";

            var details = string.Join("\n", workerDetails.Select(w => $"  {w}"));
            return $"Active workers: {activeCount}\n{details}";
        }

        private async Task<string> StopAllWorkersAsync()
        {
            var stoppedCount = await _workManager.StopAllWorkersAsync();
            return $"Stopped {stoppedCount} worker(s)";
        }

        private async Task<string> ShowHelpAsync()
        {
            var commands = GetAvailableCommands();
            var help = string.Join("\n", commands.Select(kvp => $"  {kvp.Key}: {kvp.Value}"));
            return $"Available commands:\n{help}";
        }
    }

}
