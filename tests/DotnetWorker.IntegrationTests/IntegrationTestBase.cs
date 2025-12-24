using DotnetWorker.Application;
using DotnetWorker.Infrastructure;
using DotnetWorker.WorkerService.Configurations;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotnetWorker.IntegrationTests;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    private IHost _host = null!;
    private IServiceScope _scope = null!;

    protected ISender Sender => _scope.ServiceProvider.GetRequiredService<ISender>();

    protected IServiceProvider Services => _scope.ServiceProvider;

    public async Task InitializeAsync()
    {
        var builder = Host.CreateApplicationBuilder();

        // Setup services
        builder.AddLoggerConfiguration();
        builder.Services.AddMediatorConfiguration();
        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructureServices(builder.Configuration);

        _host = builder.Build();
        _scope = _host.Services.CreateScope();

        // Ensure a clean database for each test run
        var context = _scope.ServiceProvider.GetRequiredService<DotnetWorker.Infrastructure.Data.AppDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _scope.Dispose();
        if (_host != null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
    }
}
