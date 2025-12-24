using DotnetWorker.Domain.WebsitesCheckers;

namespace DotnetWorker.UnitTests.DomainTests.WebsitesCheckers;

public class WebsiteCheckerUpdateCheckInterval
{
    [Fact]
    public void UpdateCheckInterval_ChangesInterval()
    {
        // Arrange
        var checker = WebsiteChecker.Create(WebsiteCheckerId.From(Guid.NewGuid()), WebsiteUrl.From("https://example.com"), CheckIntervalInMinutes.From(5), DateTime.UtcNow);

        // Act
        checker.UpdateCheckInterval(CheckIntervalInMinutes.From(10));

        // Assert
        Assert.Equal(CheckIntervalInMinutes.From(10), checker.CheckIntervalInMinutes);
    }
}
