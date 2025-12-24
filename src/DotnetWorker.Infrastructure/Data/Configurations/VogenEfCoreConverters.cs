using DotnetWorker.Domain.WebsitesCheckers;
using Vogen;

namespace DotnetWorker.Infrastructure.Data.Configurations;

[EfCoreConverter<WebsiteCheckerId>]
[EfCoreConverter<WebsiteUrl>]
[EfCoreConverter<CheckIntervalInMinutes>]
internal partial class VogenEfCoreConverters;
