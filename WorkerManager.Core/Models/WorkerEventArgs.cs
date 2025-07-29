namespace WorkerManager.Core.Models
{
    public class WorkerEventArgs : EventArgs
    {
        public int WorkerId { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
