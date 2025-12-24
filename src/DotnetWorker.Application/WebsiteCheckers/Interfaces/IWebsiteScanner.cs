namespace DotnetWorker.Application.WebsiteCheckers.Interfaces;

public interface IWebsiteScanner
{
    Task<WebsiteScanResult> IsWebsiteUpAsync(string url, CancellationToken cancellationToken);

    public sealed record WebsiteScanResult(bool IsUp, int StatusCode, TimeSpan ResponseTime);
}
