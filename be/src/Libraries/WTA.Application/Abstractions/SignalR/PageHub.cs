using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using WTA.Application.Abstractions.EventBus;

namespace WTA.Application.Abstractions.SignalR;

public class PageHub : Hub
{
    private readonly ILogger<PageHub> _logger;
    private readonly IEventPublisher _eventPublisher;
    private readonly object balanceLock = new object();
    public static long Count { get; private set; }

    public PageHub(ILogger<PageHub> logger, IEventPublisher eventPublisher)
    {
        this._logger = logger;
        this._eventPublisher = eventPublisher;
    }

    public override Task OnConnectedAsync()
    {
        var userName = Context.GetHttpContext()?.User.Identity?.Name;
        this._logger.LogInformation($"{Context.ConnectionId} has connected: {userName}");
        this.Groups.AddToGroupAsync(Context.ConnectionId, Context.ConnectionId);
        if (!string.IsNullOrEmpty(userName))
        {
            this.Groups.AddToGroupAsync(Context.ConnectionId, userName);
        }
        this.Clients.Group(Context.ConnectionId).SendAsync("Connected", Context.ConnectionId);
        lock (balanceLock)
        {
            Count++;
        }
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        this._logger.LogInformation($"{Context.ConnectionId} has disconnected: {exception}");
        lock (balanceLock)
        {
            Count--;
        }
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
