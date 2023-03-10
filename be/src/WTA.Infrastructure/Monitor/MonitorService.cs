using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using WTA.Application.Abstractions;
using WTA.Application.Services.Monitor;

namespace WTA.Infrastructure.Monitor;

[Service<IMonitorService>(ServiceLifetime.Singleton)]
public class MonitorService : IMonitorService, IDisposable
{
    private readonly IMonitorService _monitorService;

    public MonitorService(IServiceProvider serviceProvider)
    {
        this._monitorService = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? new WindowsService() : new LinuxService();
    }

    public void Dispose()
    {
        this._monitorService.Dispose();
    }

    public MonitorModel GetStatus()
    {
        return this._monitorService.GetStatus();
    }
}
