using Ardalis.Result;
using Ardalis.SharedKernel;
using DotnetWorker.Application.Common;
using DotnetWorker.Application.WebsiteCheckers.Dtos;
using DotnetWorker.Domain.WebsitesCheckers;
using DotnetWorker.Domain.WebsitesCheckers.Specifications;
using Mediator;

namespace DotnetWorker.Application.WebsiteCheckers.Queries;

public sealed record ListWebsiteCheckersQuery(
    int PageNumber,
    int PageSize)
    : IQuery<Result<PaginatedData<WebsiteCheckerDto>>>;

public sealed class ListWebsiteCheckersQueryHandler(
    IReadRepository<WebsiteChecker> repository)
    : IQueryHandler<ListWebsiteCheckersQuery, Result<PaginatedData<WebsiteCheckerDto>>>
{
    async ValueTask<Result<PaginatedData<WebsiteCheckerDto>>> IQueryHandler<ListWebsiteCheckersQuery, Result<PaginatedData<WebsiteCheckerDto>>>.Handle(ListWebsiteCheckersQuery query, CancellationToken cancellationToken)
    {
        var spec = new ListWebsiteCheckerSpec(query.PageNumber, query.PageSize);
        var totalItems = await repository.CountAsync(spec, cancellationToken);
        var websiteCheckers = await repository.ListAsync(spec, cancellationToken);

        var websiteCheckerDtos = websiteCheckers.Select(websiteChecker => new WebsiteCheckerDto(
            websiteChecker.Id.Value,
            websiteChecker.Url.Value,
            websiteChecker.CheckIntervalInMinutes.Value,
            websiteChecker.Status.Name,
            websiteChecker.CreatedAtUtc,
            websiteChecker.LastCheckedAtUtc)).ToList();

        var paginatedData = new PaginatedData<WebsiteCheckerDto>(
            websiteCheckerDtos,
            totalItems,
            query.PageNumber,
            query.PageSize);

        return Result.Success(paginatedData);
    }
}
