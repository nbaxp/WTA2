using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using WTA.Application.Abstractions.SignalR;
using WTA.Application.Services.Monitor;

namespace WTA.Infrastructure.Monitor;

public class BaseService
{
    public Process CurrentProcess = Process.GetCurrentProcess();

    public BaseService()
    {
    }

    public MonitorModel CreateModel()
    {
        var addresses = Dns.GetHostAddresses(Dns.GetHostName())
            .Where(o => o.AddressFamily == AddressFamily.InterNetwork)
            .Select(o => o.ToString())
            .Where(o => !o.StartsWith("127."))
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
            HeapSizeBytes = gcMemoryInfo.HeapSizeBytes,
            ProcessMemory = CurrentProcess.WorkingSet64,
            OnlineUsers= PageHub.Count,
            HandleCount = CurrentProcess.HandleCount
        };
         return model;//CurrentProcess.Threads.h
    }
}
