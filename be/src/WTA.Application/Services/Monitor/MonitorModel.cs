using System.ComponentModel.DataAnnotations;

namespace WTA.Application.Services.Monitor;

public class MonitorModel
{
    [Display]
    public double CpuUsage { get; set; }

    [Display]
    public string FrameworkDescription { get; set; } = null!;

    [Display]
    public double MemoryUsage { get; set; }

    [Display]
    public string OSArchitecture { get; set; } = null!;

    [Display]
    public string OSDescription { get; set; } = null!;

    [Display]
    public string ProcessArchitecture { get; set; } = null!;

    [Display]
    public int ProcessCount { get; set; }

    [Display]
    public double? ProcessCpuLoad { get; set; }

    [Display]
    public string ProcessName { get; set; } = null!;

    [Display]
    public int ProcessorCount { get; set; }

    [Display]
    public float ProcessMemory { get; set; }

    [Display]
    public DateTimeOffset ServerTime { get; set; }
    [Display]
    public string ServicePack { get; set; }

    [Display]
    public float SpeedReceived { get; set; }

    [Display]
    public float SpeedSent { get; set; }

    [Display]
    public int ThreadCount { get; set; }

    [Display]
    public long TotalPhysicalMemory { get; set; }
    [Display]
    public double TotalSeconds { get; set; }

    [Display]
    public string UserName { get; set; } = null!;
    [Display]
    public float DiskRead { get; set; }
    [Display]
    public float DiskWrite { get; set; }
}
