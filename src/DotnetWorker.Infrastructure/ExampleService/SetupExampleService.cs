using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetWorker.Infrastructure.ExampleService;

internal static class SetupExampleService
{
    public static IServiceCollection AddExampleService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptionsWithValidateOnStart<ExampleServiceConfig>()
            .BindConfiguration(nameof(ExampleServiceConfig))
            .ValidateDataAnnotations();

        services.AddScoped<IExampleService, ExampleService>();

        return services;
    }
}
