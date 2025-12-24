using Ardalis.SharedKernel;
using DotnetWorker.Application.Common;
using DotnetWorker.Infrastructure.Common;
using DotnetWorker.Infrastructure.Data;
using DotnetWorker.Infrastructure.Data.Interceptors;
using DotnetWorker.Infrastructure.ExampleService;
using DotnetWorker.Infrastructure.ScanWebsites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetWorker.Infrastructure;

public static class InfrastructureServicesCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        ConfigureDatabase(services);
        ConfigureCommonServices(services);

        // Other services
        services.AddScanWebsiteJob();
        services.AddExampleService(configuration);

        return services;
    }

    private static void ConfigureDatabase(IServiceCollection services)
    {
        // Interceptors
        services.AddScoped<EventDispatcherInterceptor>();

        // Domain Event Dispatcher
        services.AddScoped<IDomainEventDispatcher, MediatorDomainEventDispatcher>();

        // DbContext
        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var eventDispatcherInterceptor = serviceProvider.GetRequiredService<EventDispatcherInterceptor>();

            // In memory database for simplicity, replace with your database provider
            options.UseInMemoryDatabase("DotnetWorkerInMemoryDb");

            options.AddInterceptors(eventDispatcherInterceptor);
        });

        // Repositories
        services
            .AddScoped(typeof(IRepository<>), typeof(EfCoreRepository<>))
            .AddScoped(typeof(IReadRepository<>), typeof(EfCoreRepository<>));
    }

    private static void ConfigureCommonServices(IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();
    }
}
