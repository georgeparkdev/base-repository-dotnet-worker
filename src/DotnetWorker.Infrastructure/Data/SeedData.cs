using DotnetWorker.Domain.WebsitesCheckers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotnetWorker.Infrastructure.Data;

#pragma warning disable S1075 // URIs should not be hardcoded
public static class SeedData
{
    private static readonly List<string> WebsiteUrls = new List<string>
    {
        "https://google.com",
        "https://dotnet.microsoft.com",
    };

    public static async Task SeedDataAsync(IHost host, CancellationToken cancellationToken = default)
    {
        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await InitializeAsync(context, cancellationToken);
    }

    public static async Task InitializeAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        await SeedWebsitesAsync(context, cancellationToken);
    }

    private static async Task SeedWebsitesAsync(AppDbContext context, CancellationToken cancellationToken)
    {
        if (await context.WebsitesCheckers.AnyAsync(cancellationToken))
        {
            return;
        }

        var websites = WebsiteUrls
            .Select(url => WebsiteChecker.Create(
                WebsiteCheckerId.From(Guid.NewGuid()),
                WebsiteUrl.From(url),
                CheckIntervalInMinutes.From(1),
                DateTime.UtcNow))
            .ToList();

        await context.WebsitesCheckers.AddRangeAsync(websites, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
#pragma warning restore S1075 // URIs should not be hardcoded
