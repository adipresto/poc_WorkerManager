using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainAplikasi.ScopedProcess.Interfaces
{
    public interface IScopedProcessingService
    {
        Task DoWork(int workerId, CancellationToken cancellationToken);
    }
}
