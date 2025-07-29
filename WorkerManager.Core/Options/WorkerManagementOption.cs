namespace WorkerManager.Core.Options
{
    // Configuration Options
    public class WorkerManagementOptions
    {
        public int InitialWorkerCount { get; set; } = 1;
        public int MinWorkerCount { get; set; } = 1;
        public int MaxWorkerCount { get; set; } = 10;
        public TimeSpan WorkInterval { get; set; } = TimeSpan.FromSeconds(5);
        public TimeSpan CleanupInterval { get; set; } = TimeSpan.FromMinutes(1);
        public bool EnableAutoCleanup { get; set; } = true;
    }
}
