using Ardalis.SharedKernel;
using DotnetWorker.Application.WebsiteCheckers.Commands;
using DotnetWorker.Domain.WebsitesCheckers;
using DotnetWorker.Domain.WebsitesCheckers.Specifications;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotnetWorker.Infrastructure.ScanWebsites;

// Uses ScanWebsiteCommand sent via IMediator to scan websites periodically
public sealed class ScanWebsiteJob(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<ScanWebsiteJobConfiguration> options,
    ILogger<ScanWebsiteJob> logger) : BackgroundService
{
    private readonly ScanWebsiteJobConfiguration _config = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await ProcessWebsitesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while scanning websites.");
            }
        }
    }

    private async Task ProcessWebsitesAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IReadRepository<WebsiteChecker>>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        int pageNumber = 1;
        int batchSize = _config.BatchSize;

        while (!stoppingToken.IsCancellationRequested)
        {
            var spec = new ListWebsiteCheckerSpec(pageNumber, batchSize);
            var websites = await repository.ListAsync(spec, stoppingToken);

            if (websites.Count == 0)
            {
                break;
            }

            foreach (var websiteId in websites.Select(w => w.Id))
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                try
                {
                    await mediator.Send(new ScanWebsiteCommand(websiteId), stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error scanning website {WebsiteId}", websiteId);
                }
            }

            pageNumber++;
        }
    }
}
