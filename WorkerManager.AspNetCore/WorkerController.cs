using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WorkerManager.Core.Contexts;
using WorkerManager.Core.Interfaces;

namespace WorkerManager.AspNetCore
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkerController : ControllerBase
    {
        private readonly IWorkerCommandProcessor _commandProcessor;
        private readonly IWorkManager _workManager;

        public WorkerController(IWorkerCommandProcessor commandProcessor, IWorkManager workManager)
        {
            _commandProcessor = commandProcessor;
            _workManager = workManager;
        }

        [HttpPost("command/{command}")]
        public async Task<IActionResult> ProcessCommand(string command)
        {
            var result = await _commandProcessor.ProcessCommandAsync(command);
            return Ok(new { Command = command, Result = result, Timestamp = DateTime.UtcNow });
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            var workerDetails = _workManager.GetWorkerDetails();
            var activeCount = _workManager.GetActiveWorkerCount();

            return Ok(new
            {
                ActiveWorkerCount = activeCount,
                Workers = workerDetails,
                Timestamp = DateTime.UtcNow
            });
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddWorker([FromQuery] int count = 1)
        {
            await _workManager.AddWorkerAsync(count);
            return Ok(new { Message = $"Added {count} worker(s)", ActiveCount = _workManager.GetActiveWorkerCount() });
        }

        [HttpPost("add-with-parameter")]
        public async Task<IActionResult> AddWorkerWithParameter([FromBody] object parameter, [FromQuery] int count = 1)
        {
            await _workManager.AddWorkerAsync(parameter, count);
            return Ok(new { Message = $"Added {count} worker(s) with parameter", ActiveCount = _workManager.GetActiveWorkerCount() });
        }

        [HttpPost("add-with-context")]
        public async Task<IActionResult> AddWorkerWithContext([FromBody] WorkerContext context, [FromQuery] int count = 1)
        {
            await _workManager.AddWorkerAsync(context, count);
            return Ok(new { Message = $"Added {count} worker(s) with context", ActiveCount = _workManager.GetActiveWorkerCount() });
        }

        [HttpPost("remove")]
        public async Task<IActionResult> RemoveWorker([FromQuery] int count = 1)
        {
            var removed = await _workManager.RemoveWorkerAsync(count);
            return Ok(new { Message = $"Removal requested for {count} worker(s)", Success = removed, ActiveCount = _workManager.GetActiveWorkerCount() });
        }

        [HttpPost("stop")]
        public async Task<IActionResult> StopAllWorkers()
        {
            var stoppedCount = await _workManager.StopAllWorkersAsync();
            return Ok(new { Message = $"Stopped {stoppedCount} worker(s)" });
        }

        [HttpGet("commands")]
        public IActionResult GetAvailableCommands()
        {
            var commands = _commandProcessor.GetAvailableCommands();
            return Ok(commands);
        }

    }
}
