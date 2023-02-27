using Microsoft.Extensions.DependencyInjection;

namespace WTA.Application.Abstractions;

[AttributeUsage(AttributeTargets.Class)]
public class ServiceAttribute<T> : Attribute, IServiceAttribute
{
    public ServiceAttribute(ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        ServiceType = typeof(T);
        Lifetime = lifetime;
    }

    public Type ServiceType { get; }
    public ServiceLifetime Lifetime { get; }
}
