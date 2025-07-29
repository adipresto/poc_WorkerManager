using WorkerManager.Core.Interfaces;
using Microsoft.Extensions.Logging;
using WorkerManager.Core.Contexts;
using WorkerManager.Core.Enums;

namespace WorkerManager.Core
{
    // Command Processor
    public class WorkerCommandProcessor : IWorkerCommandProcessor
    {
        private readonly IWorkManager _workManager;
        private readonly ILogger<WorkerCommandProcessor> _logger;
        private readonly Dictionary<string, Func<Task<string>>> _commands;
        private readonly Dictionary<string, Func<Task<string>>> _commandsForAddWorker;

        private ProcessorStateEnums _processorStateEnums = ProcessorStateEnums.MAINMENU;

        public WorkerCommandProcessor(IWorkManager workManager, ILogger<WorkerCommandProcessor> logger)
        {
            _workManager = workManager;
            _logger = logger;

            _commands = new Dictionary<string, Func<Task<string>>>(StringComparer.OrdinalIgnoreCase)
            {
                ["A"] = ShowAvailableWorkerTypesAsync,
                ["ADD"] = ShowAvailableWorkerTypesAsync,
                ["D"] = RemoveWorkerAsync,
                ["DELETE"] = RemoveWorkerAsync,
                ["REMOVE"] = RemoveWorkerAsync,
                ["L"] = ListWorkersAsync,
                ["LIST"] = ListWorkersAsync,
                ["STATUS"] = ListWorkersAsync,
                ["S"] = StopAllWorkersAsync,
                ["STOP"] = StopAllWorkersAsync,
                ["H"] = ShowHelpAsync,
                ["HELP"] = ShowHelpAsync,
            };

            _commandsForAddWorker = new Dictionary<string, Func<Task<string>>>(StringComparer.OrdinalIgnoreCase)
            {
                ["0"] = AddWorkerAsync,
                ["1"] = AddParameterizedStringWorkerAsync,
                ["2"] = AddParameterizedIntWorkerAsync,
                ["B"] = BackToMainMenu,
                ["BACK"] = BackToMainMenu,
                ["H"] = ShowHelpAsync,
                ["HELP"] = ShowHelpAsync,
            };
        }

        public async Task<string> ProcessCommandAsync(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                return "Please provide a command. Type 'H' for help.";

            try
            {
                var cmd = command.Trim().ToUpper();

                switch (_processorStateEnums)
                {
                    case ProcessorStateEnums.ADDMENU:
                        if (_commandsForAddWorker.TryGetValue(cmd, out var commandAddFunc))
                        {
                            _processorStateEnums = ProcessorStateEnums.MAINMENU;
                            return await commandAddFunc();
                        }
                        break;
                    default:
                        if (_commands.TryGetValue(cmd, out var commandFunc))
                        {
                            return await commandFunc();
                        }
                        break;
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

        public Dictionary<string, string> GetAvailableWorkerTypes()
        {
            return new Dictionary<string, string>
            {
                ["0"] = "Add a new default worker",
                ["1"] = "Add a new string worker",
                ["2"] = "Add a new numerical worker",
                ["B/BACK"] = "Return to Main Menu",
                ["H/HELP"] = "Show this help"
            };
        }

        private async Task<string> AddWorkerAsync()
        {
            await _workManager.AddWorkerAsync(1);
            var activeCount = _workManager.GetActiveWorkerCount();
            return $"Added 1 worker. Active workers: {activeCount}";
        }

        private async Task<string> AddParameterizedStringWorkerAsync()
        {
            await _workManager.AddWorkerAsync("Ini dan itu", 1);
            var activeCount = _workManager.GetActiveWorkerCount();
            return $"Added 1 worker. Active workers: {activeCount}";
        }

        private async Task<string> AddParameterizedIntWorkerAsync()
        {
            await _workManager.AddWorkerAsync(100, 1);
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
            switch (_processorStateEnums) 
            {
                case ProcessorStateEnums.ADDMENU:
                    return "You can choose worker to process either string or numerical at this moment.";
            }
            var commands = GetAvailableCommands();
            var help = string.Join("\n", commands.Select(kvp => $"  {kvp.Key}: {kvp.Value}"));
            return $"Available commands:\n{help}";
        }

        private async Task<string> ShowAvailableWorkerTypesAsync()
        {
            _processorStateEnums = ProcessorStateEnums.ADDMENU;
            var commands = GetAvailableWorkerTypes();
            var types = string.Join("\n", commands.Select(kvp => $"  {kvp.Key}: {kvp.Value}"));
            return $"Choose available worker:\n{types}";
        }

        private async Task<string> BackToMainMenu()
        {
            _processorStateEnums = ProcessorStateEnums.MAINMENU;
            return await ShowHelpAsync();
        }
    }

}
