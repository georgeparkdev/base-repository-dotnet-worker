using Vogen;

namespace DotnetWorker.Domain.WebsitesCheckers;

[ValueObject<Guid>]
public readonly partial struct WebsiteCheckerId
{
    private static Validation Validate(Guid value) =>
        value == Guid.Empty
            ? Validation.Invalid("WebsiteCheckerId cannot be an empty GUID.")
            : Validation.Ok;
}
