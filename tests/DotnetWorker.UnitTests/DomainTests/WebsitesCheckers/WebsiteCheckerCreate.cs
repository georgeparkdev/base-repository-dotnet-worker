using DotnetWorker.Domain.WebsitesCheckers;

namespace DotnetWorker.UnitTests.DomainTests.WebsitesCheckers;

public class WebsiteCheckerCreate
{
    [Fact]
    public void Create_SetsExpectedProperties()
    {
        // Arrange
        var id = WebsiteCheckerId.From(Guid.NewGuid());
        var url = WebsiteUrl.From("https://example.com");
        var interval = CheckIntervalInMinutes.From(5);
        var createdAt = new DateTime(2025, 12, 24, 12, 0, 0, DateTimeKind.Utc);

        // Act
        var checker = WebsiteChecker.Create(id, url, interval, createdAt);

        // Assert
        Assert.Equal(id, checker.Id);
        Assert.Equal(url, checker.Url);
        Assert.Equal(WebsiteStatus.Unknown, checker.Status);
        Assert.Equal(interval, checker.CheckIntervalInMinutes);
        Assert.Equal(createdAt, checker.CreatedAtUtc);
        Assert.Null(checker.LastCheckedAtUtc);
    }
}
