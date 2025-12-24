using System.Diagnostics;
using DotnetWorker.Application.WebsiteCheckers.Interfaces;

namespace DotnetWorker.Infrastructure.ScanWebsites;

public class WebsiteScanner(IHttpClientFactory httpClientFactory) : IWebsiteScanner
{
    public async Task<IWebsiteScanner.WebsiteScanResult> IsWebsiteUpAsync(string url, CancellationToken cancellationToken)
    {
        try
        {
            using var client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(10);

            var stopwatch = Stopwatch.StartNew();
            var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            stopwatch.Stop();

            return new IWebsiteScanner.WebsiteScanResult(
                IsUp: response.IsSuccessStatusCode,
                StatusCode: (int)response.StatusCode,
                ResponseTime: stopwatch.Elapsed);
        }
        catch (Exception)
        {
            return new IWebsiteScanner.WebsiteScanResult(
                IsUp: false,
                StatusCode: 0,
                ResponseTime: TimeSpan.Zero);
        }
    }
}
