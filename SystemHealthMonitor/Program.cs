using Serilog;
using SystemHealthMonitor.Services;

namespace SystemHealthMonitor;

class Program
{
    static void Main(string[] args)
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: "logs/system-health-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                retainedFileCountLimit: 30)
            .CreateLogger();

        try
        {
            Log.Information("=".PadRight(80, '='));
            Log.Information("System Health Monitor Application");
            Log.Information("=".PadRight(80, '='));
            
            var healthMonitor = new HealthMonitorService(Log.Logger);
            healthMonitor.Start();
            
            // Keep the application running
            Thread.Sleep(Timeout.Infinite);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
