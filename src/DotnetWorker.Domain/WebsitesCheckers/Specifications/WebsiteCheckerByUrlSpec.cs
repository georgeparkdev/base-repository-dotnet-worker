using Ardalis.Specification;

namespace DotnetWorker.Domain.WebsitesCheckers.Specifications;

public sealed class WebsiteCheckerByUrlSpec : Specification<WebsiteChecker>
{
    public WebsiteCheckerByUrlSpec(WebsiteUrl url) =>
        Query
            .Where(websiteChecker => websiteChecker.Url == url);
}
