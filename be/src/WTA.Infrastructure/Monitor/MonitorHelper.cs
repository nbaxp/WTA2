using System.Diagnostics;
using System.Runtime.InteropServices;
using WTA.Application.Services.Monitor;

namespace WTA.Infrastructure.Monitor;

public static class MonitorHelper
{
    public static MonitorModel CreateModel()
    {
        var model = new MonitorModel
        {
            ServerTime = DateTimeOffset.UtcNow,
            UserName = Environment.UserName,
            OSArchitecture = RuntimeInformation.OSArchitecture.ToString(),
            OSDescription = RuntimeInformation.OSDescription,
            ProcessCount = Process.GetProcesses().Length,
            ThreadCount = Process.GetProcesses().Sum(o => o.Threads.Count),
            ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
            ProcessorCount = Environment.ProcessorCount,
            FrameworkDescription = RuntimeInformation.FrameworkDescription,
        };
        return model;
    }
}
