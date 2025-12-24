using DotnetWorker.Domain.WebsitesCheckers;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace DotnetWorker.Infrastructure.Data.Configurations;

internal sealed class WebsiteCheckerIdValueGenerator : ValueGenerator<WebsiteCheckerId>
{
    public override bool GeneratesTemporaryValues => false;

    public override WebsiteCheckerId Next(EntityEntry entry) =>
        WebsiteCheckerId.From(Guid.NewGuid());
}
