using MainAplikasi;
using Microsoft.Extensions.Hosting;
using WorkerManager.Console;
using WorkerManager.Core;

var builder = Host.CreateDefaultBuilder(args);

var host = builder.ConfigureServices(services =>
{
    services.AddWorkerManagement<MainWorkerService>(options =>
    {
        options.InitialWorkerCount = 1;
        options.MaxWorkerCount = 3;
    });

    services.AddWorkerConsoleInterface();
}).Build();

await host.RunAsync();