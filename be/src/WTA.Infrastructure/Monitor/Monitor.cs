using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using WTA.Application.Services.Monitor;

namespace WTA.Infrastructure.Monitor;

public class Monitor// : IHostedService
{
    public static Process CurrentProcess = Process.GetCurrentProcess();

    public static MonitorModel CreateModel()
    {
        var addresses = Dns.GetHostAddresses(Dns.GetHostName())
            .Where(o => o.AddressFamily == AddressFamily.InterNetwork || o.AddressFamily == AddressFamily.InterNetworkV6)
            .Select(o => o.ToString())
            .ToArray();
        var gcMemoryInfo = GC.GetGCMemoryInfo();
        var model = new MonitorModel
        {
            ServerTime = DateTimeOffset.UtcNow,
            UserName = Environment.UserName,
            OSArchitecture = RuntimeInformation.OSArchitecture.ToString(),
            OSDescription = RuntimeInformation.OSDescription,
            ProcessCount = Process.GetProcesses().Length,
            ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
            ProcessorCount = Environment.ProcessorCount,
            FrameworkDescription = RuntimeInformation.FrameworkDescription,
            ProcessName = CurrentProcess.ProcessName,
            processId = CurrentProcess.Id,
            ProcessFileName = Environment.ProcessPath!,
            HostName = Dns.GetHostName(),
            HostAddresses = string.Join(',', addresses),
            ProcessThreadCount = CurrentProcess.Threads.Count,
            ProcessStartTime = CurrentProcess.StartTime,
            ProcessArguments = Environment.CommandLine,
            GCTotalMemory = GC.GetTotalMemory(false),
            FinalizationPendingCount = gcMemoryInfo.FinalizationPendingCount,
            HeapSizeBytes = gcMemoryInfo.HeapSizeBytes
        };
        return model;
    }
}
