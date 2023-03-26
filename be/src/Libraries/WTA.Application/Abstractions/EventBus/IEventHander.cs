namespace WTA.Application.Abstractions.EventBus;

public interface IEventHander<T>
{
    Task Handle(T data);
}
