namespace WorkerManager.Core.Interfaces
{
    public interface IWorkerCommandProcessor
    {
        Task<string> ProcessCommandAsync(string command);
        Dictionary<string, string> GetAvailableCommands();
    }
}
