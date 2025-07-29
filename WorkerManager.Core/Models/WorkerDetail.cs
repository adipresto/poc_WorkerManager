namespace WorkerManager.Core.Models
{
    public class WorkerDetail
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public string Status { get; set; } = "Running";
        public TimeSpan RunningDuration => DateTime.UtcNow - StartTime;

        public override string ToString()
        {
            return $"Worker {Id}: {Status}, Started: {StartTime:HH:mm:ss}, Running for: {RunningDuration:hh\\:mm\\:ss}";
        }
    }
}
