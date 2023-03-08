using System.ComponentModel.DataAnnotations;

namespace WTA.Application.Services.Monitor;

public class MonitorModel
{
    [Display]
    public DateTimeOffset ServerTime { get; set; }

    [Display]
    public string OSArchitecture { get; set; } = null!;

    [Display]
    public string OSDescription { get; set; } = null!;

    [Display]
    public string ProcessArchitecture { get; set; } = null!;
    public int ProcessorCount { get; set; }
    public double CpuLoad { get; set; }
    public ulong AvailablePhysicalMemory { get; set; }
    public ulong UsedPhysicalMemory { get; set; }
}
