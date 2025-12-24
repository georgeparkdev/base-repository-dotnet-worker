using Ardalis.Result;
using DotnetWorker.Application.Common;
using DotnetWorker.Application.WebsiteCheckers.Commands;
using DotnetWorker.Domain.WebsitesCheckers;
using DotnetWorker.Domain.WebsitesCheckers.Specifications;
using Moq;

namespace DotnetWorker.UnitTests.ApplicationTests.WebsiteCheckers.Commands;

public class CreateWebsiteCheckerTests
{
    [Fact]
    public async Task Handler_Should_CreateNewWebsiteChecker_WhenUrlNotExists()
    {
        // Arrange
        var now = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        _ = dateTimeProviderMock.SetupGet(d => d.UtcNow).Returns(now);

        var repositoryMock = new Mock<Ardalis.SharedKernel.IRepository<WebsiteChecker>>();
        _ = repositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<WebsiteCheckerByUrlSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WebsiteChecker?)null);

        WebsiteChecker? captured = null;
        _ = repositoryMock.Setup(r => r.AddAsync(It.IsAny<WebsiteChecker>(), It.IsAny<CancellationToken>()))
            .Callback<WebsiteChecker, CancellationToken>((w, ct) => captured = w)
            .Returns<WebsiteChecker, CancellationToken>((w, ct) => Task.FromResult(w));

        var handler = new CreateWebsiteCheckerHandler(dateTimeProviderMock.Object, repositoryMock.Object);
        var command = new CreateWebsiteCheckerCommand(WebsiteUrl.From("https://example.com"), CheckIntervalInMinutes.From(5));

        // Act
        var result = await ((Mediator.ICommandHandler<CreateWebsiteCheckerCommand, Result<CreateWebsiteCheckerResult>>)handler)
            .Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotNull(captured);
        Assert.Equal(command.Url, captured!.Url);
        Assert.Equal(command.CheckIntervalInMinutes, captured.CheckIntervalInMinutes);
        Assert.Equal(now, captured.CreatedAtUtc);
        Assert.Equal(captured.Id.Value, result.Value.Id);
        repositoryMock.Verify(r => r.AddAsync(It.IsAny<WebsiteChecker>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_ReturnError_WhenUrlAlreadyExists()
    {
        // Arrange
        var existing = WebsiteChecker.Create(WebsiteCheckerId.From(Guid.NewGuid()), WebsiteUrl.From("https://example.com"), CheckIntervalInMinutes.From(5), DateTime.UtcNow);
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        var repositoryMock = new Mock<Ardalis.SharedKernel.IRepository<WebsiteChecker>>();
        _ = repositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<WebsiteCheckerByUrlSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var handler = new CreateWebsiteCheckerHandler(dateTimeProviderMock.Object, repositoryMock.Object);
        var command = new CreateWebsiteCheckerCommand(WebsiteUrl.From("https://example.com"), CheckIntervalInMinutes.From(5));

        // Act
        var result = await ((Mediator.ICommandHandler<CreateWebsiteCheckerCommand, Result<CreateWebsiteCheckerResult>>)handler)
            .Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Errors);
        Assert.Contains(result.Errors, e => e == "A WebsiteChecker with the same URL already exists.");
        repositoryMock.Verify(r => r.AddAsync(It.IsAny<WebsiteChecker>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_PropagateException_WhenAddAsyncThrows()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        _ = dateTimeProviderMock.SetupGet(d => d.UtcNow).Returns(now);

        var repositoryMock = new Mock<Ardalis.SharedKernel.IRepository<WebsiteChecker>>();
        _ = repositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<WebsiteCheckerByUrlSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WebsiteChecker?)null);
        _ = repositoryMock.Setup(r => r.AddAsync(It.IsAny<WebsiteChecker>(), It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("db error"));

        var handler = new CreateWebsiteCheckerHandler(dateTimeProviderMock.Object, repositoryMock.Object);
        var command = new CreateWebsiteCheckerCommand(WebsiteUrl.From("https://example.com"), CheckIntervalInMinutes.From(5));

        // Act & Assert
        _ = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await ((Mediator.ICommandHandler<CreateWebsiteCheckerCommand, Result<CreateWebsiteCheckerResult>>)handler)
                .Handle(command, CancellationToken.None));
    }
}
