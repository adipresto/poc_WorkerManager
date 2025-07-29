using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerManager.Core.Contexts
{
    // Context untuk Worker dengan multiple parameters
    public class WorkerContext
    {
        public Dictionary<string, object> Parameters { get; set; } = new();
        public int WorkerId { get; set; }
        public DateTime StartTime { get; set; }

        public T GetParameter<T>(string key, T defaultValue = default)
        {
            if (Parameters.TryGetValue(key, out var value) && value is T typedValue)
                return typedValue;
            return defaultValue;
        }

        public void SetParameter<T>(string key, T value)
        {
            Parameters[key] = value;
        }
    }
}
