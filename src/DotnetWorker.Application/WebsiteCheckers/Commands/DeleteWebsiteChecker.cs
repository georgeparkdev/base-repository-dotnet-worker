using Ardalis.Result;
using Ardalis.SharedKernel;
using DotnetWorker.Domain.WebsitesCheckers;
using Mediator;

namespace DotnetWorker.Application.WebsiteCheckers.Commands;

public sealed record DeleteWebsiteCheckerCommand(WebsiteCheckerId WebsiteCheckerId) : ICommand<Result<DeleteWebsiteCheckerResult>>;

public sealed record DeleteWebsiteCheckerResult(bool Deleted);

public sealed class DeleteWebsiteCheckerHandler(
    IRepository<WebsiteChecker> repository)
    : ICommandHandler<DeleteWebsiteCheckerCommand, Result<DeleteWebsiteCheckerResult>>
{
    async ValueTask<Result<DeleteWebsiteCheckerResult>> ICommandHandler<DeleteWebsiteCheckerCommand, Result<DeleteWebsiteCheckerResult>>.Handle(DeleteWebsiteCheckerCommand command, CancellationToken cancellationToken)
    {
        var websiteChecker = await repository.GetByIdAsync(command.WebsiteCheckerId, cancellationToken);
        if (websiteChecker is null)
        {
            return Result.NotFound();
        }

        await repository.DeleteAsync(websiteChecker, cancellationToken);
        return Result.Success(new DeleteWebsiteCheckerResult(true));
    }
}
