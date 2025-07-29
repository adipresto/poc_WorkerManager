using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainAplikasi.Models
{
    // Custom Data Classes
    public class MyCustomData
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }

    public class WorkerConfig
    {
        public int BatchSize { get; set; } = 10;
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
    }
}
