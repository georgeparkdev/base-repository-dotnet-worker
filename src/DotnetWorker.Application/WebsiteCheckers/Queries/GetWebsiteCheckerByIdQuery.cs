using Ardalis.Result;
using Ardalis.SharedKernel;
using DotnetWorker.Application.WebsiteCheckers.Dtos;
using DotnetWorker.Domain.WebsitesCheckers;
using Mediator;

namespace DotnetWorker.Application.WebsiteCheckers.Queries;

public sealed record GetWebsiteCheckerByIdQuery(Guid WebsiteCheckerId) : IQuery<Result<WebsiteCheckerDto>>;

public sealed class GetWebsiteCheckerByIdQueryHandler(
    IReadRepository<WebsiteChecker> repository)
    : IQueryHandler<GetWebsiteCheckerByIdQuery, Result<WebsiteCheckerDto>>
{
    async ValueTask<Result<WebsiteCheckerDto>> IQueryHandler<GetWebsiteCheckerByIdQuery, Result<WebsiteCheckerDto>>.Handle(GetWebsiteCheckerByIdQuery query, CancellationToken cancellationToken)
    {
        var websiteChecker = await repository.GetByIdAsync(query.WebsiteCheckerId, cancellationToken);
        if (websiteChecker is null)
        {
            return Result.NotFound();
        }

        var result = new WebsiteCheckerDto(
            websiteChecker.Id.Value,
            websiteChecker.Url.Value,
            websiteChecker.CheckIntervalInMinutes.Value,
            websiteChecker.Status.Name,
            websiteChecker.CreatedAtUtc,
            websiteChecker.LastCheckedAtUtc);

        return Result.Success(result);
    }
}
