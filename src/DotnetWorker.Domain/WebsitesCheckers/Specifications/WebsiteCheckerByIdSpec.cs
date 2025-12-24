using Ardalis.Specification;

namespace DotnetWorker.Domain.WebsitesCheckers.Specifications;

public sealed class WebsiteCheckerByIdSpec : Specification<WebsiteChecker>
{
    public WebsiteCheckerByIdSpec(WebsiteCheckerId id) =>
        Query
            .Where(websiteChecker => websiteChecker.Id == id);
}
