using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;

namespace WTA.Application.Abstractions;

[AttributeUsage(AttributeTargets.Class)]
public class ServiceAttribute<T> : Attribute, IServiceAttribute
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="lifetime"></param>
    /// <param name="platform"></param>
    public ServiceAttribute(ServiceLifetime lifetime = ServiceLifetime.Transient, PlatformType platform = PlatformType.All)
    {
        this.ServiceType = typeof(T);
        this.Lifetime = lifetime;
        if (platform == PlatformType.Windows)
        {
            this.Platform = OSPlatform.Windows;
        }
        else if (platform == PlatformType.Linux)
        {
            this.Platform = OSPlatform.Linux;
        }
        else if (platform == PlatformType.OSX)
        {
            this.Platform = OSPlatform.OSX;
        }
    }

    public Type ServiceType { get; }
    public ServiceLifetime Lifetime { get; }
    public OSPlatform? Platform { get; }
}
