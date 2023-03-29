using Microsoft.AspNetCore.SignalR;

public class PageHub : Hub
{
    private readonly ILogger<PageHub> _logger;

    public PageHub(ILogger<PageHub> logger)
    {
        this._logger = logger;
    }

    public override Task OnConnectedAsync()
    {
        this._logger.LogInformation($"{Context.ConnectionId} has connected");
        this.Groups.AddToGroupAsync(Context.ConnectionId, Context.ConnectionId);
        this.Clients.Group(Context.ConnectionId).SendAsync("connected", Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        this._logger.LogInformation($"{Context.ConnectionId} has disconnected: {exception}");
        return base.OnDisconnectedAsync(exception);
    }
    public void SetTail(bool enabled, string connectionId)
    {
        var groupName = "tail";
        if (enabled)
        {
            this.Groups.AddToGroupAsync(connectionId, groupName);
        }
        else
        {
            this.Groups.RemoveFromGroupAsync(connectionId, groupName);
        }
    }
}
