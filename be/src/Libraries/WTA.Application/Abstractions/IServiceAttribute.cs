using Microsoft.Extensions.DependencyInjection;

namespace WTA.Application.Abstractions;

public interface IServiceAttribute
{
    ServiceLifetime Lifetime { get; }
    Type ServiceType { get; }
    PlatformType PlatformType { get; }
}
