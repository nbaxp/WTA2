namespace WTA.Infrastructure.Monitor;

//[Service<IMonitorService>(ServiceLifetime.Singleton)]
//public class MonitorService : IMonitorService, IDisposable
//{
//    private readonly IMonitorService _monitorService;

//    public MonitorService(IServiceProvider serviceProvider)
//    {
//        this._monitorService = serviceProvider.GetRequiredService<IMonitorService>();// RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? new WindowsService() : new LinuxService();
//    }

//    public void Dispose()
//    {
//        this._monitorService.Dispose();
//    }

//    public MonitorModel GetStatus()
//    {
//        return this._monitorService.GetStatus();
//    }
//}
