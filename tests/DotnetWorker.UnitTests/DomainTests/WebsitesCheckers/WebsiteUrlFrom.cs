using DotnetWorker.Domain.WebsitesCheckers;

namespace DotnetWorker.UnitTests.DomainTests.WebsitesCheckers;

public class WebsiteUrlFrom
{
    [Theory]
    [InlineData("http://example.com", "http://example.com")]
    [InlineData("https://example.com", "https://example.com")]
    [InlineData("  https://example.com  ", "https://example.com")]
    [InlineData("  https://example.com", "https://example.com")]
    [InlineData("https://example.com  ", "https://example.com")]
    public void Creates_GivenValidValue_Successfully(string input, string expected)
    {
        // Arrange

        // Act
        var result = WebsiteUrl.From(input);

        // Assert
        Assert.Equal(expected, result.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid-url")]
    [InlineData("domain-only.com")]
    public void ThrowsException_GivenInvalidValue(string? input)
    {
        // Arrange

        // Act

        // Assert
        Assert.Throws<Vogen.ValueObjectValidationException>(() => WebsiteUrl.From(input!));
    }
}
