using WTA.Application.Services.Monitor;

namespace WTA.Application.Abstractions;

public interface IMonitorService : IDisposable
{
    MonitorModel GetStatus();
}
