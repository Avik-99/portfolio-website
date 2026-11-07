using Serilog;
using Serilog.Events;

namespace Portfolio.Infrastructure.Logging;

/// <summary>
/// Configures the application-wide Serilog logger with Console and rolling File sinks.
/// Wired from the host (Portfolio.Web) via a single line in Program.cs.
/// </summary>
public static class SerilogConfigurator
{
    public static void ConfigureDefault(LoggerConfiguration configuration)
    {
        configuration
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                Path.Combine("logs", "application-.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");
    }
}
