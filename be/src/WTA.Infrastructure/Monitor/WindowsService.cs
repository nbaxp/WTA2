using System.Diagnostics;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Management;
using System.Runtime.Versioning;
using Microsoft.Extensions.DependencyInjection;
using WTA.Application.Abstractions;
using WTA.Application.Extensions;
using WTA.Application.Services.Monitor;

namespace WTA.Infrastructure.Monitor;

/// <summary>
/// 单例
/// </summary>
[Service<IMonitorService>(ServiceLifetime.Singleton, PlatformType.Windows)]
[SupportedOSPlatform("windows")]
public class WindowsService : BaseService, IMonitorService
{
    private readonly PerformanceCounter CPUCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
    private readonly PerformanceCounter ThreadCounter = new PerformanceCounter("Process", "Thread Count", "_Total");
    private readonly PerformanceCounter ProcessDistReadCounter;
    private readonly PerformanceCounter ProcessDistWriteCounter;
    private readonly PerformanceCounter ProcessCPUCounter;
    private readonly PerformanceCounter MemoryCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
    private readonly PerformanceCounterCategory NetworkInterfaceCategory = new PerformanceCounterCategory("Network Interface");
    private readonly PerformanceCounter PhysicalDiskReadCounter = new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", "_Total");
    private readonly PerformanceCounter PhysicalDiskWriteCounter = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", "_Total");
    private readonly List<PerformanceCounter> ReceivedCounters = new List<PerformanceCounter>();
    private readonly List<PerformanceCounter> SentCounters = new List<PerformanceCounter>();
    private string[] Names = Array.Empty<string>();

    public WindowsService()
    {
        this.ProcessDistReadCounter = new PerformanceCounter("Process", "IO Read Bytes/sec", base.CurrentProcess.ProcessName);
        this.ProcessDistWriteCounter = new PerformanceCounter("Process", "IO Write Bytes/sec", base.CurrentProcess.ProcessName);
        this.ProcessCPUCounter = new PerformanceCounter("Process", "% Processor Time", base.CurrentProcess.ProcessName);
        UpdateNetWorkCounters();
    }

    public void Dispose()
    {
        CPUCounter.Dispose();
        ThreadCounter.Dispose();
        ProcessDistReadCounter.Dispose();
        ProcessDistWriteCounter.Dispose();
        ProcessCPUCounter.Dispose();
        MemoryCounter.Dispose();
        PhysicalDiskReadCounter.Dispose();
        PhysicalDiskWriteCounter.Dispose();
        ReceivedCounters.ForEach(o => o.Dispose());
        SentCounters.ForEach(o => o.Dispose());
    }

    public MonitorModel GetStatus()
    {
        var model = base.CreateModel();
        model.CpuUsage = CPUCounter.NextValue() / 100;
        model.ProcessCpuUsage = ProcessCPUCounter.NextValue() / 100 / Environment.ProcessorCount;
        model.MemoryUsage = MemoryCounter.NextValue() / 100;
        this.UpdateNetWorkCounters();
        model.NetReceived = ReceivedCounters.Sum(o => o.NextValue());
        model.NetSent = SentCounters.Sum(o => o.NextValue());
        model.DiskRead = PhysicalDiskReadCounter.NextValue();
        model.DiskWrite = PhysicalDiskWriteCounter.NextValue();
        model.ProcessDiskRead = ProcessDistReadCounter.NextValue();
        model.ProcessDiskWrite = ProcessDistWriteCounter.NextValue();
        model.ThreadCount = (int)ThreadCounter.NextValue();
        using var mc = new ManagementClass("Win32_PhysicalMemory");
        foreach (ManagementObject item in mc.GetInstances())
        {
            model.TotalMemory += item.Properties["Capacity"].Value.ToString()!.ToLong();
            item.Dispose();
        }
        return model;
    }

    private void UpdateNetWorkCounters()
    {
        var names = NetworkInterfaceCategory.GetInstanceNames();
        if (!Enumerable.SequenceEqual(Names, names))
        {
            Names = names;
            ReceivedCounters.ForEach(o => o.Dispose());
            ReceivedCounters.Clear();
            SentCounters.ForEach(o => o.Dispose());
            SentCounters.Clear();
            names.ForEach(name =>
            {
                ReceivedCounters.Add(new PerformanceCounter("Network Interface", "Bytes Received/sec", name));
                SentCounters.Add(new PerformanceCounter("Network Interface", "Bytes Sent/sec", name));
            });
        }
    }
}
