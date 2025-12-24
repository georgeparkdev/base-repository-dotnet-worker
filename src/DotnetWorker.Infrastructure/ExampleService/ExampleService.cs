using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotnetWorker.Infrastructure.ExampleService;

public sealed class ExampleService(
    ILogger<ExampleService> logger,
    IOptions<ExampleServiceConfig> config)
    : IExampleService
{
    public void DoSomething()
    {
        logger.LogInformation("Doing something with setting: {SomeSetting}", config.Value.SomeSetting);
    }
}
