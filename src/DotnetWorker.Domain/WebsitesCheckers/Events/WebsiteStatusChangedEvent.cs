using Ardalis.SharedKernel;

namespace DotnetWorker.Domain.WebsitesCheckers.Events;

public sealed class WebsiteStatusChangedEvent(WebsiteUrl url, WebsiteStatus status) : DomainEventBase
{
    public WebsiteUrl Url { get; init; } = url;

    public WebsiteStatus Status { get; init; } = status;
}
