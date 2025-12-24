using DotnetWorker.Domain.WebsitesCheckers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotnetWorker.Infrastructure.Data.Configurations;

internal sealed class WebsiteCheckerConfiguration : IEntityTypeConfiguration<WebsiteChecker>
{
    public const string TableName = "WebsitesCheckers";

    public void Configure(EntityTypeBuilder<WebsiteChecker> builder)
    {
        builder.ToTable(TableName);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasVogenConversion();

        builder.Property(x => x.Url)
            .HasVogenConversion()
            .HasMaxLength(WebsiteUrl.MaxLength)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion(
                p => p.Value,
                p => WebsiteStatus.FromValue(p))
            .IsRequired();

        builder.Property(x => x.CheckIntervalInMinutes)
            .HasVogenConversion()
            .IsRequired();
    }
}
