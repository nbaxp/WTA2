namespace WTA.Application.Abstractions.EventBus;

public interface IEventPublisher
{
    Task Publish<T>(T data);
}
