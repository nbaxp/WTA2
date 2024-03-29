using System.ComponentModel.DataAnnotations;

namespace WTA.Application.Services.Monitor;

public class MonitorModel
{
    [Display]
    public double CpuUsage { get; set; }

    [Display]
    public float DiskRead { get; set; }

    [Display]
    public float DiskWrite { get; set; }

    [Display]
    public long FinalizationPendingCount { get; set; }

    [Display]
    public string FrameworkDescription { get; set; } = null!;

    [Display]
    public long GCTotalMemory { get; set; }

    [Display]
    public long HeapSizeBytes { get; set; }

    [Display]
    public string HostAddresses { get; set; } = null!;

    [Display]
    public string HostName { get; set; } = null!;

    [Display]
    public double MemoryUsage { get; set; }

    [Display]
    public string OSArchitecture { get; set; } = null!;

    [Display]
    public string OSDescription { get; set; } = null!;

    [Display]
    public string ProcessArchitecture { get; set; } = null!;

    [Display]
    public string ProcessArguments { get; set; } = null!;

    [Display]
    public int ProcessCount { get; set; }

    [Display]
    public double? ProcessCpuUsage { get; set; }

    [Display]
    public float ProcessDiskRead { get; set; }

    [Display]
    public float ProcessDiskWrite { get; set; }

    [Display]
    public string ProcessFileName { get; set; } = null!;

    [Display]
    public int processId { get; set; }

    [Display]
    public float ProcessMemory { get; set; }

    [Display]
    public string ProcessName { get; set; } = null!;

    [Display]
    public int ProcessorCount { get; set; }

    [Display]
    public TimeSpan ProcessRunTime { get; set; }

    [Display]
    public DateTime ProcessStartTime { get; set; }

    [Display]
    public int ProcessThreadCount { get; set; }

    [Display]
    public DateTimeOffset ServerTime { get; set; }

    [Display]
    public string ServicePack { get; set; } = null!;

    [Display]
    public float NetReceived { get; set; }

    [Display]
    public float NetSent { get; set; }

    [Display]
    public int ThreadCount { get; set; }

    [Display]
    public long TotalMemory { get; set; }

    [Display]
    public double TotalSeconds { get; set; }

    [Display]
    public string UserName { get; set; } = null!;

    [Display]
    public long OnlineUsers { get; set; }
    [Display]
    public int HandleCount { get; set; }
}
