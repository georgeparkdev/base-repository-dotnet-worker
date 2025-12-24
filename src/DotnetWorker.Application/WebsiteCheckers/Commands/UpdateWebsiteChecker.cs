using Ardalis.Result;
using Ardalis.SharedKernel;
using DotnetWorker.Domain.WebsitesCheckers;
using Mediator;

namespace DotnetWorker.Application.WebsiteCheckers.Commands;

public sealed record UpdateWebsiteCheckerCommand(
    WebsiteCheckerId WebsiteCheckerId,
    CheckIntervalInMinutes CheckIntervalInMinutes) : ICommand<Result>;

public sealed class UpdateWebsiteCheckerHandler(
    IRepository<WebsiteChecker> repository)
    : ICommandHandler<UpdateWebsiteCheckerCommand, Result>
{
    async ValueTask<Result> ICommandHandler<UpdateWebsiteCheckerCommand, Result>.Handle(UpdateWebsiteCheckerCommand command, CancellationToken cancellationToken)
    {
        var websiteChecker = await repository.GetByIdAsync(command.WebsiteCheckerId, cancellationToken);
        if (websiteChecker is null)
        {
            return Result.NotFound();
        }

        websiteChecker.UpdateCheckInterval(command.CheckIntervalInMinutes);
        await repository.UpdateAsync(websiteChecker, cancellationToken);

        return Result.Success();
    }
}
