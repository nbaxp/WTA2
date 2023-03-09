using System.Diagnostics;
using WTA.Application.Abstractions;
using WTA.Application.Services.Monitor;

namespace WTA.Infrastructure.Monitor;

public class WindowsService : IMonitorService
{
    private IServiceProvider _serviceProvider;

    public WindowsService(IServiceProvider serviceProvider)
    {
        this._serviceProvider = serviceProvider;
    }

    public void Dispose()
    {
    }

    public MonitorModel GetStatus()
    {
        var model = MonitorHelper.CreateModel();
        using var CPUCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        model.CpuUsage = CPUCounter.NextValue();
        model.CpuUsage = CPUCounter.NextValue();
        Debug.WriteLine(model.CpuUsage);
        return model;
    }
}
