using MainAplikasi.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkerManager.Core.Interfaces;

namespace MainAplikasi
{
    public class ParameterizedWorkerService : IScopedParameterizedWorkerService
    {
        private readonly ILogger<ParameterizedWorkerService> _logger;

        public ParameterizedWorkerService(ILogger<ParameterizedWorkerService> logger)
        {
            _logger = logger;
        }

        public async Task DoWorkAsync(object parameter, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing parameter of type: {Type}", parameter?.GetType().Name ?? "null");

            switch (parameter)
            {
                case string str:
                    _logger.LogInformation("String parameter: {Value}", str);
                    break;
                case int num:
                    _logger.LogInformation("Integer parameter: {Value}", num);
                    break;
                case MyCustomData data:
                    _logger.LogInformation("Custom data: {Name}, {Value}", data.Name, data.Value);
                    break;
                default:
                    _logger.LogInformation("Unknown parameter type");
                    break;
            }

            await Task.Delay(1000, cancellationToken);
        }
    }
}
