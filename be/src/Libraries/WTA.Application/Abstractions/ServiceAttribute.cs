using Microsoft.Extensions.DependencyInjection;

namespace WTA.Application.Abstractions;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class BaseServiceAttribute : Attribute
{
    public ServiceLifetime Lifetime { get; set; }
    public PlatformType PlatformType { get; set; }
    public Type ServiceType { get; set; } = null!;
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ServiceAttribute<T> : BaseServiceAttribute
{
    public ServiceAttribute(ServiceLifetime lifetime = ServiceLifetime.Transient, PlatformType platformType = PlatformType.All)
    {
        this.ServiceType = typeof(T);
        this.Lifetime = lifetime;
        PlatformType = platformType;
    }
}
