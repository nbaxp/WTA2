using System.Diagnostics;
using System.Linq.Dynamic.Core;
using System.Management;
using WTA.Application.Abstractions;
using WTA.Application.Extensions;
using WTA.Application.Services.Monitor;

namespace WTA.Infrastructure.Monitor;

/// <summary>
/// 单例
/// </summary>
public class WindowsService : IMonitorService
{
    private readonly PerformanceCounter CPUCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
    private readonly PerformanceCounter ThreadCounter = new PerformanceCounter("Process", "Thread Count", "_Total");
    private readonly PerformanceCounter ProcessDistReadCounter = new PerformanceCounter("Process", "IO Read Bytes/sec", Monitor.CurrentProcess.ProcessName);
    private readonly PerformanceCounter ProcessDistWriteCounter = new PerformanceCounter("Process", "IO Write Bytes/sec", Monitor.CurrentProcess.ProcessName);
    private readonly PerformanceCounter ProcessCPUCounter = new PerformanceCounter("Process", "% Processor Time", Monitor.CurrentProcess.ProcessName);
    private readonly PerformanceCounter MemoryCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
    private readonly PerformanceCounterCategory NetworkInterfaceCategory = new PerformanceCounterCategory("Network Interface");
    private readonly PerformanceCounter PhysicalDiskReadCounter = new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", "_Total");
    private readonly PerformanceCounter PhysicalDiskWriteCounter = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", "_Total");
    private readonly List<PerformanceCounter> ReceivedCounters = new List<PerformanceCounter>();
    private readonly List<PerformanceCounter> SentCounters = new List<PerformanceCounter>();
    private string[] Names = new string[] { };

    public WindowsService()
    {
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
        var model = Monitor.CreateModel();
        model.CpuUsage = CPUCounter.NextValue() / 100;
        model.ProcessCpuLoad = ProcessCPUCounter.NextValue() / 100 / Environment.ProcessorCount;
        model.MemoryUsage = MemoryCounter.NextValue() / 100;
        model.ProcessMemory = Monitor.CurrentProcess.PrivateMemorySize64;
        this.UpdateNetWorkCounters();
        model.SpeedReceived = ReceivedCounters.Sum(o => o.NextValue());
        model.SpeedSent = SentCounters.Sum(o => o.NextValue());
        model.DiskRead = PhysicalDiskReadCounter.NextValue();
        model.DiskWrite = PhysicalDiskWriteCounter.NextValue();
        model.ProcessDiskRead = ProcessDistReadCounter.NextValue();
        model.ProcessDiskWrite = ProcessDistWriteCounter.NextValue();
        model.ThreadCount = (int)ThreadCounter.NextValue();
        using var mc = new ManagementClass("Win32_PhysicalMemory");
        foreach (ManagementObject item in mc.GetInstances())
        {
            model.TotalPhysicalMemory += long.Parse(item.Properties["Capacity"].Value.ToString());
            item.Dispose();
        }
        Debug.WriteLine(model.CpuUsage.ToString("F2") + "," +
            model.MemoryUsage.ToString("F2") + "," +
            (model.SpeedReceived / 1024).ToString("F2") + "," +
            (model.SpeedSent / 1024).ToString("F2") + "," +
            (model.DiskRead / 1024).ToString("F2") + "," +
            (model.DiskWrite / 1024).ToString("F2"));
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
