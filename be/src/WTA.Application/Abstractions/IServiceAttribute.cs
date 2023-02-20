using Microsoft.Extensions.DependencyInjection;

namespace WTA.Core.Abstractions;

public interface IServiceAttribute
{
    ServiceLifetime Lifetime { get; }
    Type ServiceType { get; }
}
