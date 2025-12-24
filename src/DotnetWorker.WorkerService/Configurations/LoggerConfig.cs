using Serilog;

namespace DotnetWorker.WorkerService.Configurations;

public static class LoggerConfig
{
    public static IHostApplicationBuilder AddLoggerConfiguration(this IHostApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog();

        return builder;
    }
}
