using DotnetWorker.Domain.WebsitesCheckers;
using Microsoft.EntityFrameworkCore;

namespace DotnetWorker.Infrastructure.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<WebsiteChecker> WebsitesCheckers => Set<WebsiteChecker>();

    public override int SaveChanges() =>
        SaveChangesAsync().GetAwaiter().GetResult();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
    }
}
