using System.Diagnostics;
using Serilog;
using SystemHealthMonitor.Models;

namespace SystemHealthMonitor.Services;

public class HealthMonitorService
{
    private readonly ILogger _logger;
    private readonly PerformanceCounter? _cpuCounter;
    private Timer? _timer;

    public HealthMonitorService(ILogger logger)
    {
        _logger = logger;
        
        // Initialize CPU counter (works on Windows, alternative for Linux)
        try
        {
            if (OperatingSystem.IsWindows())
            {
                _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            }
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Could not initialize CPU performance counter");
        }
    }

    public void Start()
    {
        _logger.Information("System Health Monitor started");
        _logger.Information("Monitoring system metrics every 10 seconds...");
        
        // Create timer that fires every 10 seconds
        _timer = new Timer(CheckSystemHealth, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
        
        Console.WriteLine("Press Ctrl+C to stop monitoring...");
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            Stop();
        };
    }

    private void CheckSystemHealth(object? state)
    {
        try
        {
            var metrics = CollectMetrics();
            LogMetrics(metrics);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error collecting system metrics");
        }
    }

    private SystemMetrics CollectMetrics()
    {
        var metrics = new SystemMetrics
        {
            Timestamp = DateTime.UtcNow
        };

        // Get CPU usage
        if (OperatingSystem.IsWindows() && _cpuCounter != null)
        {
            metrics.CpuUsagePercent = _cpuCounter.NextValue();
        }
        else
        {
            // For Linux/Mac, use Process-based approximation
            metrics.CpuUsagePercent = GetCpuUsageLinux();
        }

        // Get memory usage
        var currentProcess = Process.GetCurrentProcess();
        metrics.MemoryUsedMB = currentProcess.WorkingSet64 / (1024.0 * 1024.0);
        
        // Get total system memory (cross-platform)
        var gcMemoryInfo = GC.GetGCMemoryInfo();
        metrics.MemoryTotalMB = gcMemoryInfo.TotalAvailableMemoryBytes / (1024.0 * 1024.0);
        
        if (metrics.MemoryTotalMB > 0)
        {
            metrics.MemoryUsagePercent = (metrics.MemoryUsedMB / metrics.MemoryTotalMB) * 100;
        }

        return metrics;
    }

    private double GetCpuUsageLinux()
    {
        // This is a simplified approach for non-Windows systems
        // Returns current process CPU usage as a percentage
        var currentProcess = Process.GetCurrentProcess();
        var startTime = DateTime.UtcNow;
        var startCpuUsage = currentProcess.TotalProcessorTime;

        Thread.Sleep(100); // Small delay to measure CPU

        var endTime = DateTime.UtcNow;
        var endCpuUsage = currentProcess.TotalProcessorTime;

        var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
        var totalMsPassed = (endTime - startTime).TotalMilliseconds;
        var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);

        return cpuUsageTotal * 100;
    }

    private void LogMetrics(SystemMetrics metrics)
    {
        _logger.Information(
            "System Metrics - Timestamp: {Timestamp}, CPU Usage: {CpuUsage:F2}%, " +
            "Memory Used: {MemoryUsed:F2} MB, Total Memory: {TotalMemory:F2} MB, " +
            "Memory Usage: {MemoryUsagePercent:F2}%",
            metrics.Timestamp,
            metrics.CpuUsagePercent,
            metrics.MemoryUsedMB,
            metrics.MemoryTotalMB,
            metrics.MemoryUsagePercent
        );

        // Also write to console for user feedback
        Console.WriteLine(metrics.ToString());
    }

    public void Stop()
    {
        _logger.Information("Stopping System Health Monitor...");
        _timer?.Dispose();
        _cpuCounter?.Dispose();
        _logger.Information("System Health Monitor stopped");
        Environment.Exit(0);
    }
}
