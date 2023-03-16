using Microsoft.Extensions.DependencyInjection;

namespace WTA.Application.Abstractions;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ServiceAttribute<T> : Attribute, IServiceAttribute
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="lifetime"></param>
    /// <param name="platform"></param>
    public ServiceAttribute(ServiceLifetime lifetime = ServiceLifetime.Transient, PlatformType platformType = PlatformType.All)
    {
        this.ServiceType = typeof(T);
        this.Lifetime = lifetime;
        PlatformType = platformType;
    }

    public ServiceLifetime Lifetime { get; }
    public PlatformType PlatformType { get; }
    public Type ServiceType { get; }
}
