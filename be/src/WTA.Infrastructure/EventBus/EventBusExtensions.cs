using Microsoft.Extensions.DependencyInjection;
using WTA.Application;
using WTA.Application.Abstractions.EventBus;

namespace WTA.Infrastructure.EventBus;

public static class EventBusExtensions
{
    public static void AddEventBus(this IServiceCollection services)
    {
        services.AddEventBus<DefaultEventPublisher>();
    }

    public static void AddEventBus<T>(this IServiceCollection services) where T : class, IEventPublisher
    {
        services.AddTransient<IEventPublisher, T>();
        WebApp.ModuleAssemblies?
            .Where(o => o.GetTypes().Any(o => o.IsAssignableFrom(typeof(IEventHander<>))))
            .SelectMany(o => o.GetTypes())
            .Where(t => t.GetInterfaces().Any(o => o.IsGenericType && o.GetGenericTypeDefinition() == typeof(IEventHander<>)))
            .ToList()
            .ForEach(type =>
            {
                type.GetInterfaces()
                .Where(o => o.IsGenericType && o.GetGenericTypeDefinition() == typeof(IEventHander<>)).ToList()
                .ForEach(o => services.AddTransient(o, type));
            });
    }
}
