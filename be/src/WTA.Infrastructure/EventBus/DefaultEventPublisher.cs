using Microsoft.Extensions.DependencyInjection;
using WTA.Application.Abstractions;
using WTA.Application.Abstractions.EventBus;

namespace WTA.Infrastructure.EventBus;

[Service<IEventPublisher>]
public class DefaultEventPublisher : IEventPublisher
{
    private readonly IServiceProvider _applicationServices;

    public DefaultEventPublisher(IServiceProvider applicationServices)
    {
        this._applicationServices = applicationServices;
    }

    public async Task Publish<T>(T data)
    {
        using var scope = _applicationServices.CreateScope();
        var subscribers = scope.ServiceProvider.GetServices<IEventHander<T>>().ToList();
        foreach (var item in subscribers)
        {
            try
            {
                await item.Handle(data).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception($"{typeof(T).Name}", ex);
            }
        }
    }
}
