using DotnetWorker.Application.WebsiteCheckers.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetWorker.Infrastructure.ScanWebsites;

public static class SetupScanWebsiteJob
{
    public static IServiceCollection AddScanWebsiteJob(this IServiceCollection services)
    {
        services.AddHostedService<ScanWebsiteJob>();
        services.AddScoped<IWebsiteScanner, WebsiteScanner>();
        return services;
    }
}
