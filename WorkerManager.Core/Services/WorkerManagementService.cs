using WorkerManager.Core.Interfaces;
using WorkerManager.Core.Models;
using WorkerManager.Core.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace WorkerManager.Core.Services
{
    // Internal Worker Info
    internal class WorkerInfo
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
        public Task WorkerTask { get; set; }
        public string Status { get; set; } = "Running";
    }

    // Core Worker Management Service
    public class WorkerManagementService : BackgroundService, IWorkManager, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<WorkerManagementService> _logger;
        private readonly WorkerManagementOptions _options;
        private readonly ConcurrentDictionary<int, WorkerInfo> _workers;
        private readonly SemaphoreSlim _workerManagementSemaphore = new(1, 1);
        private int _nextWorkerId = 0;

        public event EventHandler<WorkerEventArgs> WorkerAdded;
        public event EventHandler<WorkerEventArgs> WorkerRemoved;
        public event EventHandler<WorkerEventArgs> WorkerCompleted;

        public WorkerManagementService(
            IServiceProvider serviceProvider,
            ILogger<WorkerManagementService> logger,
            WorkerManagementOptions options = null)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _options = options ?? new WorkerManagementOptions();
            _workers = new ConcurrentDictionary<int, WorkerInfo>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("WorkerManagementService started with {InitialCount} initial workers",
                _options.InitialWorkerCount);

            // Start initial workers
            for (int i = 0; i < _options.InitialWorkerCount; i++)
            {
                await AddWorkerAsync();
            }

            // Background cleanup task
            if (_options.EnableAutoCleanup)
            {
                _ = Task.Run(() => BackgroundCleanupAsync(stoppingToken), stoppingToken);
            }

            // Keep service running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        public async Task AddWorkerAsync(int count = 1)
        {
            await _workerManagementSemaphore.WaitAsync();
            try
            {
                for (int i = 0; i < count; i++)
                {
                    if (_workers.Count >= _options.MaxWorkerCount)
                    {
                        _logger.LogWarning("Cannot add worker. Maximum worker count ({MaxCount}) reached",
                            _options.MaxWorkerCount);
                        break;
                    }

                    var workerId = Interlocked.Increment(ref _nextWorkerId);
                    var cts = new CancellationTokenSource();

                    var workerInfo = new WorkerInfo
                    {
                        Id = workerId,
                        StartTime = DateTime.UtcNow,
                        CancellationTokenSource = cts
                    };

                    var workerTask = Task.Run(async () =>
                    {
                        try
                        {
                            await DoWorkerWorkAsync(workerId, cts.Token);
                        }
                        catch (OperationCanceledException)
                        {
                            _logger.LogDebug("Worker {WorkerId} was cancelled", workerId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Worker {WorkerId} encountered an error", workerId);
                        }
                        finally
                        {
                            if (_workers.TryGetValue(workerId, out var info))
                            {
                                info.Status = "Completed";
                                WorkerCompleted?.Invoke(this, new WorkerEventArgs
                                {
                                    WorkerId = workerId,
                                    Action = "Completed"
                                });
                            }
                        }
                    }, cts.Token);

                    workerInfo.WorkerTask = workerTask;

                    if (_workers.TryAdd(workerId, workerInfo))
                    {
                        _logger.LogInformation("Added worker {WorkerId}. Total active workers: {Count}",
                            workerId, GetActiveWorkerCount());

                        WorkerAdded?.Invoke(this, new WorkerEventArgs
                        {
                            WorkerId = workerId,
                            Action = "Added"
                        });
                    }
                }
            }
            finally
            {
                _workerManagementSemaphore.Release();
            }
        }

        public async Task<bool> RemoveWorkerAsync(int count = 1)
        {
            await _workerManagementSemaphore.WaitAsync();
            try
            {
                int removedCount = 0;
                var activeWorkers = _workers.Where(w => w.Value.Status == "Running").Take(count);

                foreach (var worker in activeWorkers)
                {
                    if (GetActiveWorkerCount() <= _options.MinWorkerCount)
                    {
                        _logger.LogWarning("Cannot remove worker. Minimum worker count ({MinCount}) must be maintained",
                            _options.MinWorkerCount);
                        break;
                    }

                    if (_workers.TryRemove(worker.Key, out var workerInfo))
                    {
                        workerInfo.Status = "Stopping";
                        workerInfo.CancellationTokenSource.Cancel();
                        removedCount++;

                        _logger.LogInformation("Removed worker {WorkerId}. Total active workers: {Count}",
                            worker.Key, GetActiveWorkerCount());

                        WorkerRemoved?.Invoke(this, new WorkerEventArgs
                        {
                            WorkerId = worker.Key,
                            Action = "Removed"
                        });
                    }
                }

                return removedCount > 0;
            }
            finally
            {
                _workerManagementSemaphore.Release();
            }
        }

        public async Task<int> StopAllWorkersAsync()
        {
            await _workerManagementSemaphore.WaitAsync();
            try
            {
                var stoppedCount = 0;
                var workersToStop = _workers.Values.Where(w => w.Status == "Running").ToList();

                foreach (var worker in workersToStop)
                {
                    worker.Status = "Stopping";
                    worker.CancellationTokenSource.Cancel();
                    stoppedCount++;
                }

                _workers.Clear();
                _logger.LogInformation("Stopped all {Count} workers", stoppedCount);
                return stoppedCount;
            }
            finally
            {
                _workerManagementSemaphore.Release();
            }
        }

        public int GetActiveWorkerCount()
        {
            return _workers.Count(w => w.Value.Status == "Running");
        }

        public List<WorkerDetail> GetWorkerDetails()
        {
            return _workers.Values
                .Where(w => w.Status != "Completed")
                .Select(w => new WorkerDetail
                {
                    Id = w.Id,
                    StartTime = w.StartTime,
                    Status = w.Status
                })
                .OrderBy(w => w.Id)
                .ToList();
        }

        private async Task DoWorkerWorkAsync(int workerId, CancellationToken stoppingToken)
        {
            _logger.LogDebug("Worker {WorkerId} started", workerId);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var workerService = scope.ServiceProvider.GetService<IScopedWorkerService>();

                    if (workerService != null)
                    {
                        await workerService.DoWorkAsync(stoppingToken);
                    }
                    else
                    {
                        // Default work if no service is registered
                        _logger.LogDebug("Worker {WorkerId} performing default work", workerId);
                        await Task.Delay(_options.WorkInterval, stoppingToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Worker {WorkerId} encountered an error during work", workerId);
                    await Task.Delay(1000, stoppingToken);
                }
            }

            _logger.LogDebug("Worker {WorkerId} stopped", workerId);
        }

        private async Task BackgroundCleanupAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_options.CleanupInterval, stoppingToken);

                    var completedWorkers = _workers.Where(w => w.Value.Status == "Completed").ToList();
                    foreach (var worker in completedWorkers)
                    {
                        _workers.TryRemove(worker.Key, out _);
                        worker.Value.CancellationTokenSource?.Dispose();
                    }

                    if (completedWorkers.Any())
                    {
                        _logger.LogDebug("Cleaned up {Count} completed workers", completedWorkers.Count);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during background cleanup");
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("WorkerManagementService is stopping");
            await StopAllWorkersAsync();
            await base.StopAsync(stoppingToken);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                _workerManagementSemaphore?.Dispose();
                foreach (var worker in _workers.Values)
                {
                    worker.CancellationTokenSource?.Dispose();
                }
            }
            base.Dispose();
        }
    }
}
