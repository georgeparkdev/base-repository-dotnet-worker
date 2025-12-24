using DotnetWorker.Domain.WebsitesCheckers;
using DotnetWorker.Domain.WebsitesCheckers.Events;

namespace DotnetWorker.UnitTests.DomainTests.WebsitesCheckers;

public class WebsiteCheckerUpdateStatus
{
    [Fact]
    public void UpdateStatus_WhenStatusChanges_UpdatesStatusAndAddsDomainEvent()
    {
        // Arrange
        var checker = WebsiteChecker.Create(WebsiteCheckerId.From(Guid.NewGuid()), WebsiteUrl.From("https://example.com"), CheckIntervalInMinutes.From(5), DateTime.UtcNow);
        var checkedAtUtc = DateTime.UtcNow;

        // Act
        checker.UpdateStatus(WebsiteStatus.Up, checkedAtUtc);

        // Assert
        Assert.Equal(WebsiteStatus.Up, checker.Status);
        Assert.Equal(checkedAtUtc, checker.LastCheckedAtUtc);

        var domainEvents = checker.DomainEvents;
        Assert.Single(domainEvents);

        var evt = Assert.IsType<WebsiteStatusChangedEvent>(domainEvents.First());
        Assert.Equal(checker.Url, evt.Url);
        Assert.Equal(checker.Status, evt.Status);
    }

    [Fact]
    public void UpdateStatus_WhenStatusSame_DoesNotAddDomainEvent_ButUpdatesLastChecked()
    {
        // Arrange
        var checker = WebsiteChecker.Create(WebsiteCheckerId.From(Guid.NewGuid()), WebsiteUrl.From("https://example.com"), CheckIntervalInMinutes.From(5), DateTime.UtcNow);
        var firstCheckedAt = DateTime.UtcNow.AddMinutes(-10);
        checker.UpdateStatus(WebsiteStatus.Up, firstCheckedAt); // create first event

        var newCheckedAt = DateTime.UtcNow;

        // Act
        checker.UpdateStatus(WebsiteStatus.Up, newCheckedAt); // same status

        // Assert
        Assert.Equal(WebsiteStatus.Up, checker.Status);
        Assert.Equal(newCheckedAt, checker.LastCheckedAtUtc);

        var domainEvents = checker.DomainEvents;

        // Only the initial event should exist (created above), not a new one
        Assert.Single(domainEvents);
    }

    [Fact]
    public void UpdateStatus_WhenStatusChangesMultipleTimes_AddsMultipleDomainEvents()
    {
        // Arrange
        var checker = WebsiteChecker.Create(WebsiteCheckerId.From(Guid.NewGuid()), WebsiteUrl.From("https://example.com"), CheckIntervalInMinutes.From(5), DateTime.UtcNow);

        // Act
        checker.UpdateStatus(WebsiteStatus.Up, DateTime.UtcNow);
        checker.UpdateStatus(WebsiteStatus.Down, DateTime.UtcNow.AddMinutes(5));

        // Assert
        Assert.Equal(WebsiteStatus.Down, checker.Status);

        var domainEvents = checker.DomainEvents;
        Assert.Equal(2, domainEvents.Count);

        var firstEvent = Assert.IsType<WebsiteStatusChangedEvent>(domainEvents.ElementAt(0));
        Assert.Equal(WebsiteStatus.Up, firstEvent.Status);

        var secondEvent = Assert.IsType<WebsiteStatusChangedEvent>(domainEvents.ElementAt(1));
        Assert.Equal(WebsiteStatus.Down, secondEvent.Status);
    }
}
