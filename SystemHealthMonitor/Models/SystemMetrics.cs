namespace SystemHealthMonitor.Models;

public class SystemMetrics
{
    public DateTime Timestamp { get; set; }
    public double CpuUsagePercent { get; set; }
    public double MemoryUsedMB { get; set; }
    public double MemoryTotalMB { get; set; }
    public double MemoryUsagePercent { get; set; }

    public override string ToString()
    {
        return $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] CPU: {CpuUsagePercent:F2}%, " +
               $"Memory: {MemoryUsedMB:F2}MB / {MemoryTotalMB:F2}MB ({MemoryUsagePercent:F2}%)";
    }
}
