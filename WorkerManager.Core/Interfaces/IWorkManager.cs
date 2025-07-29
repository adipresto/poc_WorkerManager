using WorkerManager.Core.Contexts;
using WorkerManager.Core.Models;

namespace WorkerManager.Core.Interfaces
{
    public interface IWorkManager
    {
        Task AddWorkerAsync(int count = 1);
        Task AddWorkerAsync<T>(T parameter, int count = 1);
        Task AddWorkerAsync(object parameter, int count = 1);
        Task AddWorkerAsync(WorkerContext context, int count = 1);
        Task<bool> RemoveWorkerAsync(int count = 1);
        Task<int> StopAllWorkersAsync();
        int GetActiveWorkerCount();
        List<WorkerDetail> GetWorkerDetails();
        event EventHandler<WorkerEventArgs> WorkerAdded;
        event EventHandler<WorkerEventArgs> WorkerRemoved;
        event EventHandler<WorkerEventArgs> WorkerCompleted;
    }
}
