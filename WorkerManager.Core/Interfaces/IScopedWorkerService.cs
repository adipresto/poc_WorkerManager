using WorkerManager.Core.Contexts;

namespace WorkerManager.Core.Interfaces
{
    public interface IScopedWorkerService
    {
        Task DoWorkAsync(int millisecondsDelay, CancellationToken cancellationToken);
    }

    public interface IScopedWorkerService<T>
    {
        Task DoWorkAsync(T parameter, int millisecondsDelay, CancellationToken cancellationToken);
    }

    public interface IScopedParameterizedWorkerService
    {
        Task DoWorkAsync(object parameter, int millisecondsDelay, CancellationToken cancellationToken);
    }

    public interface IScopedContextWorkerService
    {
        Task DoWorkAsync(WorkerContext context, CancellationToken cancellationToken);
    }

}
