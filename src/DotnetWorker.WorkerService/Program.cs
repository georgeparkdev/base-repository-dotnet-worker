using DotnetWorker.Application;
using DotnetWorker.Infrastructure;
using DotnetWorker.WorkerService.Configurations;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

// Setup common services
builder.AddLoggerConfiguration();
builder.Services.AddMediatorConfiguration();

// Setup layers
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

try
{
    var host = builder.Build();

    await DotnetWorker.Infrastructure.Data.SeedData.SeedDataAsync(host);

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}

public partial class Program
{
}
