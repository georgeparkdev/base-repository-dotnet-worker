using Ardalis.Result;
using Ardalis.SharedKernel;
using DotnetWorker.Application.Common;
using DotnetWorker.Domain.WebsitesCheckers;
using DotnetWorker.Domain.WebsitesCheckers.Specifications;
using Mediator;

namespace DotnetWorker.Application.WebsiteCheckers.Commands;

public sealed record CreateWebsiteCheckerCommand(WebsiteUrl Url, CheckIntervalInMinutes CheckIntervalInMinutes)
    : ICommand<Result<CreateWebsiteCheckerResult>>;

public sealed record CreateWebsiteCheckerResult(Guid Id);

public sealed class CreateWebsiteCheckerHandler(
    IDateTimeProvider dateTimeProvider,
    IRepository<WebsiteChecker> repository)
    : ICommandHandler<CreateWebsiteCheckerCommand, Result<CreateWebsiteCheckerResult>>
{
    async ValueTask<Result<CreateWebsiteCheckerResult>> ICommandHandler<CreateWebsiteCheckerCommand, Result<CreateWebsiteCheckerResult>>.Handle(CreateWebsiteCheckerCommand command, CancellationToken cancellationToken)
    {
        // Find if a WebsiteChecker with the same URL already exists
        var existingWebsiteChecker = await repository.FirstOrDefaultAsync(new WebsiteCheckerByUrlSpec(command.Url), cancellationToken);
        if (existingWebsiteChecker is not null)
        {
            return Result.Error("A WebsiteChecker with the same URL already exists.");
        }

        // Create a new WebsiteChecker
        var newWebsiteCheckerId = WebsiteCheckerId.From(Guid.NewGuid());
        var newWebsiteChecker = WebsiteChecker.Create(
            newWebsiteCheckerId,
            command.Url,
            command.CheckIntervalInMinutes,
            dateTimeProvider.UtcNow);

        // Save the new WebsiteChecker to the repository
        await repository.AddAsync(newWebsiteChecker, cancellationToken);

        // Return the result
        return Result.Success(new CreateWebsiteCheckerResult(newWebsiteCheckerId.Value));
    }
}
