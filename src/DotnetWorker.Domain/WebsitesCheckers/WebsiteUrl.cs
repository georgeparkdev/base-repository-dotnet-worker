using Vogen;

namespace DotnetWorker.Domain.WebsitesCheckers;

[ValueObject<string>]
public readonly partial struct WebsiteUrl
{
    public const int MaxLength = 128;

    private static Validation Validate(string value) =>
        !string.IsNullOrWhiteSpace(value)
            && value.Length <= MaxLength
            && Uri.IsWellFormedUriString(value, UriKind.Absolute)
                ? Validation.Ok
                : Validation.Invalid($"WebsiteUrl must be a well-formed absolute URL, not exceed {MaxLength} characters, and cannot be null or whitespace.");

    private static string NormalizeInput(string input) => input.Trim();
}
