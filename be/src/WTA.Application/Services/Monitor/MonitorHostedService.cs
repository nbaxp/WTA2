using System.Net.Sockets;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WTA.Application.Abstractions;
using WTA.Application.Abstractions.SignalR;

namespace WTA.Application.Services.Monitor;

[Service<IHostedService>]
public class MonitorHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public MonitorHostedService(IServiceProvider applicationServices)
    {
        this._serviceProvider = applicationServices;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                this.Callback();
                await Task.Delay(2 * 1000).ConfigureAwait(true);
            }
        }, cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private void Callback()
    {
        using var scope = _serviceProvider.CreateScope();
        var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<PageHub>>();
        var monitorService = scope.ServiceProvider.GetRequiredService<IMonitorService>();
        hubContext.Clients.All.SendAsync(nameof(HubExtensions.ServerToClient), "monitor", monitorService.GetStatus());
    }
}