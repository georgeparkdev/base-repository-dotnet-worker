using Ardalis.Result;
using Ardalis.SharedKernel;
using DotnetWorker.Application.Common;
using DotnetWorker.Application.WebsiteCheckers.Interfaces;
using DotnetWorker.Domain.WebsitesCheckers;
using Mediator;

namespace DotnetWorker.Application.WebsiteCheckers.Commands;

public sealed record ScanWebsiteCommand(
    WebsiteCheckerId WebsiteCheckerId)
    : ICommand<Result<ScanWebsiteResult>>;

public sealed record ScanWebsiteResult(
    DateTime UtcScannedAt,
    bool IsUp);

public sealed class ScanWebsiteHandler(
    IDateTimeProvider dateTimeProvider,
    IWebsiteScanner websiteScanner,
    IRepository<WebsiteChecker> repository)
    : ICommandHandler<ScanWebsiteCommand, Result<ScanWebsiteResult>>
{
    public async ValueTask<Result<ScanWebsiteResult>> Handle(ScanWebsiteCommand command, CancellationToken cancellationToken)
    {
        var websiteChecker = await repository.GetByIdAsync(command.WebsiteCheckerId, cancellationToken);
        if (websiteChecker is null)
        {
            return Result.NotFound();
        }

        if (websiteChecker.LastCheckedAtUtc.HasValue)
        {
            var nextAllowedCheckTime = websiteChecker.LastCheckedAtUtc.Value.AddMinutes(websiteChecker.CheckIntervalInMinutes.Value);
            if (dateTimeProvider.UtcNow < nextAllowedCheckTime)
            {
                return Result.SuccessWithMessage($"Website was checked recently. Next allowed check time is at {nextAllowedCheckTime:u}.");
            }
        }

        var scanResult = await websiteScanner.IsWebsiteUpAsync(websiteChecker.Url.Value, cancellationToken);

        websiteChecker.UpdateStatus(
            newStatus: scanResult.IsUp ? WebsiteStatus.Up : WebsiteStatus.Down,
            checkedAtUtc: dateTimeProvider.UtcNow);

        await repository.UpdateAsync(websiteChecker, cancellationToken);

        return Result.Success(new ScanWebsiteResult(
            UtcScannedAt: dateTimeProvider.UtcNow,
            IsUp: scanResult.IsUp));
    }
}
