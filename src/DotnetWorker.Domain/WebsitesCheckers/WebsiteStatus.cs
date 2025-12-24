using Ardalis.SmartEnum;

namespace DotnetWorker.Domain.WebsitesCheckers;

public sealed class WebsiteStatus : SmartEnum<WebsiteStatus>
{
    public static readonly WebsiteStatus Up = new("Up", 1);
    public static readonly WebsiteStatus Down = new("Down", 2);
    public static readonly WebsiteStatus Unknown = new("Unknown", 3);

    private WebsiteStatus(string name, int value)
        : base(name, value)
    {
    }
}
