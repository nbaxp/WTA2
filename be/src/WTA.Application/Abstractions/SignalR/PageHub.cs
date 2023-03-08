using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using WTA.Application.Abstractions.EventBus;

namespace WTA.Application.Abstractions.SignalR;

public class PageHub : Hub
{
    private readonly ILogger<PageHub> _logger;
    private readonly IEventPublisher _eventPublisher;

    public PageHub(ILogger<PageHub> logger, IEventPublisher eventPublisher)
    {
        this._logger = logger;
        this._eventPublisher = eventPublisher;
    }

    public override Task OnConnectedAsync()
    {
        this._logger.LogInformation($"{Context.ConnectionId} has connected: {Context.GetHttpContext()?.Request.QueryString}");
        this.Groups.AddToGroupAsync(Context.ConnectionId, Context.ConnectionId);
        var group = Context.GetHttpContext()?.User.Identity?.Name;
        if (!string.IsNullOrEmpty(group))
        {
            this.Groups.AddToGroupAsync(Context.ConnectionId, group);
        }
        this.Clients.Group(Context.ConnectionId).SendAsync("Connected", Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        this._logger.LogInformation($"{Context.ConnectionId} has disconnected: {exception}");
        return base.OnDisconnectedAsync(exception);
    }

    public void FromClient(string method, string message, string to, string? from = null)
    {
        _eventPublisher.Publish(new BaseEvent<SignalREvent>(new SignalREvent
        {
            Method = method,
            Message = message,
            To = to,
            From = from
        }, nameof(SignalREvent)));
    }
}
