namespace WTA.Application.Abstractions.EventBus;

public class SignalREvent
{
    public string Method { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string To { get; set; } = null!;
    public string? From { get; set; }
}
