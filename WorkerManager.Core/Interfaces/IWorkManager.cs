using WorkerManager.Core.Contexts;
using WorkerManager.Core.Models;

namespace WorkerManager.Core.Interfaces
{
    public interface IWorkManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="millisecondsDelay"></param>
        /// <returns>WorkerID for stop and remove operation</returns>
        Task<Guid> AddWorkerAsync(int count = 1, int millisecondsDelay = 5000);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter"></param>
        /// <param name="count"></param>
        /// <param name="millisecondsDelay"></param>
        /// <returns>WorkerID for stop and remove operation</returns>
        Task<Guid> AddWorkerAsync<T>(T parameter, int count = 1, int millisecondsDelay = 5000);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="count"></param>
        /// <param name="millisecondsDelay"></param>
        /// <returns>WorkerID for stop and remove operation</returns>
        Task<Guid> AddWorkerAsync(object parameter, int count = 1, int millisecondsDelay = 5000);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="count"></param>
        /// <param name="millisecondsDelay"></param>
        /// <returns>WorkerID for stop and remove operation</returns>
        Task<Guid> AddWorkerAsync(WorkerContext context, int count = 1, int millisecondsDelay = 5000);
        /// <Guid><summary>
        /// Stop and remove worker from the last added worker
        /// </summary>
        /// <param name="count">Amount of workers from last added worker</param>
        /// <returns></returns>
        Task<bool> RemoveWorkerAsync(int count = 1);
        /// <summary>
        /// Stop and remove all workers
        /// </summary>
        /// <returns></returns>
        Task<int> StopAllWorkersAsync();
        /// <summary>
        /// Stop and remove worker
        /// </summary>
        /// <param name="workerId"></param>
        /// <returns></returns>
        Task<int> StopWorkersAsync(Guid workerId);
        int GetActiveWorkerCount();
        List<WorkerDetail> GetWorkerDetails();
        event EventHandler<WorkerEventArgs> WorkerAdded;
        event EventHandler<WorkerEventArgs> WorkerRemoved;
        event EventHandler<WorkerEventArgs> WorkerCompleted;
    }
}
