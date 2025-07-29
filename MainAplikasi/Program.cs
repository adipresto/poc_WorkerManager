using MainAplikasi;
using MainAplikasi.Models;
using Microsoft.Extensions.Hosting;
using WorkerManager.Console;
using WorkerManager.Core;
using WorkerManager.Core.Options;

var builder = Host.CreateDefaultBuilder(args);

var defaultWorkerOptions = new WorkerManagementOptions() {
    InitialWorkerCount = 0,
    MaxWorkerCount = 999
};

var host = builder.ConfigureServices(services =>
{
    services.AddWorkerManagement<MainWorkerService>(options =>
    {
        options.InitialWorkerCount = 0;
        options.MaxWorkerCount = 3;
    });
    //services.AddWorkerManagement<TypedWorkerService, string>();
    services.AddParameterizedWorkerManagement<ParameterizedWorkerService>(options =>
    {
        options.InitialWorkerCount = 0;
        options.MaxWorkerCount = 10;
    });
    //services.AddContextWorkerManagement<ContextWorkerService>();

    services.AddWorkerConsoleInterface();
}).Build();

await host.RunAsync();