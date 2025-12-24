using Vogen;

namespace DotnetWorker.Domain.WebsitesCheckers;

[ValueObject<int>]
public readonly partial struct CheckIntervalInMinutes
{
    private static Validation Validate(int value) =>
        value > 0 && value <= 1440
            ? Validation.Ok
            : Validation.Invalid("CheckIntervalInMinutes must be between 1 and 1440 minutes (24 hours).");
}
