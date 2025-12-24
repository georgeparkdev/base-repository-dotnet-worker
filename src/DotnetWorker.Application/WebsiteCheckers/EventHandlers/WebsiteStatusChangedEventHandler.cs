using DotnetWorker.Domain.WebsitesCheckers.Events;
using Mediator;
using Microsoft.Extensions.Logging;

namespace DotnetWorker.Application.WebsiteCheckers.EventHandlers;

public sealed class WebsiteStatusChangedEventHandler(
    ILogger<WebsiteStatusChangedEventHandler> logger)
    : INotificationHandler<WebsiteStatusChangedEvent>
{
    public ValueTask Handle(WebsiteStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Website status changed to {Status} for URL: {Url}", notification.Status.Name, notification.Url);
        return ValueTask.CompletedTask;
    }
}
