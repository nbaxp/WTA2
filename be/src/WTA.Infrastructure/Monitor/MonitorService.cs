using System.Runtime.InteropServices;
using CZGL.SystemInfo;
using WTA.Application.Abstractions;
using WTA.Application.Services.Monitor;

namespace WTA.Infrastructure.Monitor;

[Service<IMonitorService>]
public class MonitorService : IMonitorService
{
    private static readonly bool _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    //private static readonly Process _process = Process.GetCurrentProcess();

    public MonitorModel GetStatus()
    {
        var memory = _isWindows ? MemoryHelper.GetMemoryValue() : LinuxMemory.GetMemory();
        var network = NetworkInfo.TryGetRealNetworkInfo();
        var oldRate = network.GetIpv4Speed();
        var newRate = network.GetIpv4Speed();
        var speed = NetworkInfo.GetSpeed(oldRate, newRate);
        var model = new MonitorModel
        {
            ServerTime = DateTimeOffset.UtcNow,
            OSArchitecture = RuntimeInformation.OSArchitecture.ToString(),
            OSDescription = RuntimeInformation.OSDescription,
            ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
            ProcessorCount = SystemPlatformInfo.ProcessorCount,
            CpuLoad = GetCpuLoad(),
            AvailablePhysicalMemory = memory.AvailablePhysicalMemory,
            UsedPhysicalMemory = memory.UsedPhysicalMemory,
        };
        return model;
    }

    private static double GetCpuLoad()
    {
        var v1 = _isWindows ? CPUHelper.GetCPUTime() : LinuxCPU.GetCPUTime();
        Thread.Sleep(1000);
        var v2 = _isWindows ? CPUHelper.GetCPUTime() : LinuxCPU.GetCPUTime();
        var cpuLoad = CPUHelper.CalculateCPULoad(v1, v2);
        return cpuLoad;
    }
}
