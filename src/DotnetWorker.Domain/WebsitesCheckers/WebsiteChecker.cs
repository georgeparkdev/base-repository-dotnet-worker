using Ardalis.SharedKernel;

namespace DotnetWorker.Domain.WebsitesCheckers;

public sealed class WebsiteChecker
    : EntityBase<WebsiteChecker, WebsiteCheckerId>, IAggregateRoot
{
    private WebsiteChecker()
    {
        Status = WebsiteStatus.Unknown;
    }

    private WebsiteChecker(WebsiteCheckerId id, WebsiteUrl url, CheckIntervalInMinutes checkIntervalInMinutes, DateTime createdAtUtc)
    {
        Id = id;
        Url = url;
        Status = WebsiteStatus.Unknown;
        CheckIntervalInMinutes = checkIntervalInMinutes;
        CreatedAtUtc = createdAtUtc;
    }

    public WebsiteUrl Url { get; private set; }

    public WebsiteStatus Status { get; private set; }

    public CheckIntervalInMinutes CheckIntervalInMinutes { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? LastCheckedAtUtc { get; private set; }

    public static WebsiteChecker Create(WebsiteCheckerId id, WebsiteUrl url, CheckIntervalInMinutes checkIntervalInMinutes, DateTime createdAtUtc)
    {
        return new WebsiteChecker(id, url, checkIntervalInMinutes, createdAtUtc);
    }

    public void UpdateStatus(WebsiteStatus newStatus, DateTime checkedAtUtc)
    {
        var oldStatus = Status;
        Status = newStatus;
        LastCheckedAtUtc = checkedAtUtc;

        if (oldStatus != newStatus)
        {
            var statusChangedEvent = new Events.WebsiteStatusChangedEvent(Url, Status);
            RegisterDomainEvent(statusChangedEvent);
        }
    }

    public void UpdateCheckInterval(CheckIntervalInMinutes newInterval)
    {
        CheckIntervalInMinutes = newInterval;
    }
}
