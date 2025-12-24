using DotnetWorker.Application.WebsiteCheckers.Commands;
using DotnetWorker.Domain.WebsitesCheckers;
using Xunit;

namespace DotnetWorker.IntegrationTests.WebsiteCheckers.Commands;

public class CreateWebsiteCheckerCommandTests : IntegrationTestBase
{
    [Fact]
    public async Task CreateWebsiteChecker_ShouldSucceed_WhenUrlIsUnique()
    {
        // Arrange
        var url = WebsiteUrl.From("https://example.com");
        var interval = CheckIntervalInMinutes.From(15);
        var command = new CreateWebsiteCheckerCommand(url, interval);

        // Act
        var result = await Sender.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
    }
}
