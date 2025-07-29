using WorkerManager.Core.Interfaces;
using WorkerManager.Core.Options;
using WorkerManager.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace WorkerManager.Core
{
    // Extension Methods for DI Registration
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWorkerManagement(
            this IServiceCollection services,
            Action<WorkerManagementOptions> configureOptions = null)
        {
            var options = new WorkerManagementOptions();
            configureOptions?.Invoke(options);

            services.AddSingleton(options);
            services.AddSingleton<WorkerManagementService>();
            services.AddSingleton<IWorkManager>(provider => provider.GetRequiredService<WorkerManagementService>());
            services.AddHostedService<WorkerManagementService>(provider => provider.GetRequiredService<WorkerManagementService>());
            services.AddScoped<IWorkerCommandProcessor, WorkerCommandProcessor>();

            return services;
        }

        public static IServiceCollection AddWorkerManagement<TWorkerService>(
            this IServiceCollection services,
            Action<WorkerManagementOptions> configureOptions = null)
            where TWorkerService : class, IScopedWorkerService
        {
            services.AddScoped<IScopedWorkerService, TWorkerService>();
            return services.AddWorkerManagement(configureOptions);
        }

        public static IServiceCollection AddWorkerManagement<TWorkerService, TParameter>(
            this IServiceCollection services,
            Action<WorkerManagementOptions> configureOptions = null)
            where TWorkerService : class, IScopedWorkerService<TParameter>
        {
            services.AddScoped<IScopedWorkerService<TParameter>, TWorkerService>();
            return services.AddWorkerManagement(configureOptions);
        }

        public static IServiceCollection AddParameterizedWorkerManagement<TWorkerService>(
            this IServiceCollection services,
            Action<WorkerManagementOptions> configureOptions = null)
            where TWorkerService : class, IScopedParameterizedWorkerService
        {
            services.AddScoped<IScopedParameterizedWorkerService, TWorkerService>();
            return services.AddWorkerManagement(configureOptions);
        }

        public static IServiceCollection AddContextWorkerManagement<TWorkerService>(
            this IServiceCollection services,
            Action<WorkerManagementOptions> configureOptions = null)
            where TWorkerService : class, IScopedContextWorkerService
        {
            services.AddScoped<IScopedContextWorkerService, TWorkerService>();
            return services.AddWorkerManagement(configureOptions);
        }
    }
}
