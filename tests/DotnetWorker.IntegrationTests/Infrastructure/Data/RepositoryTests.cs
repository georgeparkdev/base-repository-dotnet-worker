using System.Linq;
using Ardalis.SharedKernel;
using DotnetWorker.Domain.WebsitesCheckers;
using DotnetWorker.Domain.WebsitesCheckers.Specifications;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotnetWorker.IntegrationTests.Infrastructure.Data;

public class RepositoryTests : IntegrationTestBase
{
    [Fact]
    public async Task AddAsync_ShouldAddEntityToDatabase()
    {
        // Arrange
        var repository = Services.GetRequiredService<IRepository<WebsiteChecker>>();
        var url = WebsiteUrl.From("https://repository-test.com");
        var interval = CheckIntervalInMinutes.From(30);
        var checker = WebsiteChecker.Create(WebsiteCheckerId.From(Guid.NewGuid()), url, interval, DateTime.UtcNow);

        // Act
        await repository.AddAsync(checker);

        // Assert
        var storedChecker = await repository.GetByIdAsync(checker.Id);
        Assert.NotNull(storedChecker);
        Assert.Equal(url, storedChecker.Url);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        var repository = Services.GetRequiredService<IRepository<WebsiteChecker>>();

        // Act
        var storedChecker = await repository.GetByIdAsync(WebsiteCheckerId.From(Guid.NewGuid()));

        // Assert
        Assert.Null(storedChecker);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistChanges()
    {
        // Arrange
        var repository = Services.GetRequiredService<IRepository<WebsiteChecker>>();
        var url = WebsiteUrl.From("https://update-test.com");
        var interval = CheckIntervalInMinutes.From(30);
        var checker = WebsiteChecker.Create(WebsiteCheckerId.From(Guid.NewGuid()), url, interval, DateTime.UtcNow);
        await repository.AddAsync(checker);

        // Act
        checker.UpdateCheckInterval(CheckIntervalInMinutes.From(60));
        checker.UpdateStatus(WebsiteStatus.Down, DateTime.UtcNow);
        await repository.UpdateAsync(checker);

        // Assert
        var stored = await repository.GetByIdAsync(checker.Id);
        Assert.NotNull(stored);
        Assert.Equal(60, stored.CheckIntervalInMinutes.Value);
        Assert.Equal(WebsiteStatus.Down, stored.Status);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntityFromDatabase()
    {
        // Arrange
        var repository = Services.GetRequiredService<IRepository<WebsiteChecker>>();
        var url = WebsiteUrl.From("https://delete-test.com");
        var interval = CheckIntervalInMinutes.From(15);
        var checker = WebsiteChecker.Create(WebsiteCheckerId.From(Guid.NewGuid()), url, interval, DateTime.UtcNow);
        await repository.AddAsync(checker);

        // Act
        await repository.DeleteAsync(checker);

        // Assert
        var stored = await repository.GetByIdAsync(checker.Id);
        Assert.Null(stored);
    }

    [Fact]
    public async Task FirstOrDefaultAsync_ShouldReturnEntity_WhenSpecMatches()
    {
        // Arrange
        var repository = Services.GetRequiredService<IRepository<WebsiteChecker>>();
        var url1 = WebsiteUrl.From("https://spec-test-1.com");
        var url2 = WebsiteUrl.From("https://spec-test-2.com");
        var interval = CheckIntervalInMinutes.From(10);
        var c1 = WebsiteChecker.Create(WebsiteCheckerId.From(Guid.NewGuid()), url1, interval, DateTime.UtcNow);
        var c2 = WebsiteChecker.Create(WebsiteCheckerId.From(Guid.NewGuid()), url2, interval, DateTime.UtcNow);
        await repository.AddAsync(c1);
        await repository.AddAsync(c2);

        // Act
        var found = await repository.FirstOrDefaultAsync(new WebsiteCheckerByUrlSpec(url1));

        // Assert
        Assert.NotNull(found);
        Assert.Equal(url1, found.Url);
    }

    [Fact]
    public async Task CountAndListAsync_ShouldSupportPagination()
    {
        // Arrange
        var repository = Services.GetRequiredService<IRepository<WebsiteChecker>>();
        var interval = CheckIntervalInMinutes.From(5);
        var c1 = WebsiteChecker.Create(WebsiteCheckerId.From(Guid.NewGuid()), WebsiteUrl.From("https://page-1.com"), interval, DateTime.UtcNow.AddMinutes(-3));
        var c2 = WebsiteChecker.Create(WebsiteCheckerId.From(Guid.NewGuid()), WebsiteUrl.From("https://page-2.com"), interval, DateTime.UtcNow.AddMinutes(-2));
        var c3 = WebsiteChecker.Create(WebsiteCheckerId.From(Guid.NewGuid()), WebsiteUrl.From("https://page-3.com"), interval, DateTime.UtcNow.AddMinutes(-1));

        await repository.AddAsync(c1);
        await repository.AddAsync(c2);
        await repository.AddAsync(c3);

        // Act
        var spec = new ListWebsiteCheckerSpec(1, 2);
        var totalItems = await repository.CountAsync(spec);
        var page = (await repository.ListAsync(spec)).ToList();

        // Assert
        Assert.Equal(3, totalItems);
        Assert.Equal(2, page.Count);

        // Because spec orders by CreatedAtUtc ascending, first page item should be the oldest one
        Assert.Equal(c1.Url, page[0].Url);
    }
}
