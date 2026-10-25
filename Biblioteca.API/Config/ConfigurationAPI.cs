using Serilog;
using Serilog.Events;

namespace Biblioteca.API.Config;

public static class ConfigurationAPI
{
    public static void AddLogApi(this IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(
                path: Path.Combine(AppContext.BaseDirectory, "logs", "api-.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext}{NewLine}    {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        services.AddSerilog();
    }
}
