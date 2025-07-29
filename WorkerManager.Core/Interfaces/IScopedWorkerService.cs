using WorkerManager.Core.Contexts;

namespace WorkerManager.Core.Interfaces
{
    public interface IScopedWorkerService
    {
        Task DoWorkAsync(CancellationToken cancellationToken);
    }

    public interface IScopedWorkerService<T>
    {
        Task DoWorkAsync(T parameter, CancellationToken cancellationToken);
    }

    public interface IScopedParameterizedWorkerService
    {
        Task DoWorkAsync(object parameter, CancellationToken cancellationToken);
    }

    public interface IScopedContextWorkerService
    {
        Task DoWorkAsync(WorkerContext context, CancellationToken cancellationToken);
    }

}
