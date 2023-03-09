using System.Diagnostics;
using System.Linq.Dynamic.Core;
using System.Management;
using WTA.Application.Abstractions;
using WTA.Application.Extensions;
using WTA.Application.Services.Monitor;

namespace WTA.Infrastructure.Monitor;

public class WindowsService : IMonitorService
{
    private IServiceProvider _serviceProvider;
    private static PerformanceCounter CPUCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
    private static PerformanceCounter ProcessCPUCounter = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);
    private static PerformanceCounter MemoryCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
    private static List<PerformanceCounter> ReceivedCounters = new List<PerformanceCounter>();
    private static List<PerformanceCounter> SentCounters = new List<PerformanceCounter>();
    private static PerformanceCounter PhysicalDiskReadCounter = new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", "_Total");
    private static PerformanceCounter PhysicalDiskWriteCounter = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", "_Total");

    public WindowsService(IServiceProvider serviceProvider)
    {
        this._serviceProvider = serviceProvider;
        var networkInterfaceCategory = new PerformanceCounterCategory("Network Interface");
        networkInterfaceCategory.GetInstanceNames().ForEach(name =>
        {
            ReceivedCounters.Add(new PerformanceCounter("Network Interface", "Bytes Received/sec", name));
            SentCounters.Add(new PerformanceCounter("Network Interface", "Bytes Sent/sec", name));
        });
    }

    public void Dispose()
    {
        CPUCounter.Dispose();
        ProcessCPUCounter.Dispose();
        MemoryCounter.Dispose();
        ReceivedCounters.ForEach(o => o.Dispose());
        SentCounters.ForEach(o => o.Dispose());
        PhysicalDiskReadCounter.Dispose();
        PhysicalDiskWriteCounter.Dispose();
    }

    public MonitorModel GetStatus()
    {
        var model = MonitorHelper.CreateModel();
        model.CpuUsage = CPUCounter.NextValue() / 100;
        model.ProcessCpuLoad = ProcessCPUCounter.NextValue() / 100 / Environment.ProcessorCount;
        model.MemoryUsage = MemoryCounter.NextValue() / 100;
        model.ProcessMemory = Process.GetCurrentProcess().PrivateMemorySize64;
        model.SpeedReceived = ReceivedCounters.Sum(o => o.NextValue());
        model.SpeedSent = SentCounters.Sum(o => o.NextValue());
        model.DiskRead = PhysicalDiskReadCounter.NextValue();
        model.DiskWrite = PhysicalDiskWriteCounter.NextValue();
        using var mc = new ManagementClass("Win32_PhysicalMemory");
        foreach (ManagementObject item in mc.GetInstances())
        {
            model.TotalPhysicalMemory += long.Parse(item.Properties["Capacity"].Value.ToString());
        }
        Debug.WriteLine(model.CpuUsage.ToString("F2") + "," +
            model.MemoryUsage.ToString("F2") + "," +
            (model.SpeedReceived / 1024).ToString("F2") + "," +
            (model.SpeedSent / 1024).ToString("F2") + "," +
            (model.DiskRead / 1024).ToString("F2") + "," +
            (model.DiskWrite / 1024).ToString("F2"));
        return model;
    }
}
