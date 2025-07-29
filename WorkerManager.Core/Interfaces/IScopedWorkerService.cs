namespace WorkerManager.Core.Interfaces
{
    public interface IScopedWorkerService
    {
        Task DoWorkAsync(CancellationToken cancellationToken);
    }
}
