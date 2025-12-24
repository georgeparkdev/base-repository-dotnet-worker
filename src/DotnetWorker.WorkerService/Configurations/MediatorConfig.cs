using Ardalis.SharedKernel;

namespace DotnetWorker.WorkerService.Configurations;

public static class MediatorConfig
{
    public static IServiceCollection AddMediatorConfiguration(this IServiceCollection services)
    {
        services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;

            options.Assemblies = [
                typeof(Domain.AssemblyReference),
                typeof(Application.AssemblyReference),
                typeof(Infrastructure.AssemblyReference),
                typeof(AssemblyReference),
            ];

            options.PipelineBehaviors = [
                typeof(LoggingBehavior<,>)
            ];
        });

        return services;
    }
}
