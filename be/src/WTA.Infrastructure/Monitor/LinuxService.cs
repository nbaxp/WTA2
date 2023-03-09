using WTA.Application.Abstractions;
using WTA.Application.Services.Monitor;

namespace WTA.Infrastructure.Monitor;

internal class LinuxService : IMonitorService
{
    private IServiceProvider _serviceProvider;

    public LinuxService(IServiceProvider serviceProvider)
    {
        this._serviceProvider = serviceProvider;
    }

    public void Dispose()
    {

    }

    public MonitorModel GetStatus()
    {
        var model = MonitorHelper.CreateModel();
        return model;
    }
}
