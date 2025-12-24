using Ardalis.Specification;

namespace DotnetWorker.Domain.WebsitesCheckers.Specifications;

public sealed class ListWebsiteCheckerSpec : Specification<WebsiteChecker>
{
    public ListWebsiteCheckerSpec(int pageNumber, int pageSize)
    {
        Query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        Query
            .OrderBy(websiteChecker => websiteChecker.CreatedAtUtc);
    }
}
