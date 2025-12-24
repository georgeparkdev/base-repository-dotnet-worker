using Ardalis.Specification;

namespace DotnetWorker.Domain.WebsitesCheckers.Specifications;

public sealed class WebsiteCheckerByStatusSpec : Specification<WebsiteChecker>
{
    public WebsiteCheckerByStatusSpec(WebsiteStatus status) =>
        Query
            .Where(websiteChecker => websiteChecker.Status == status);
}
