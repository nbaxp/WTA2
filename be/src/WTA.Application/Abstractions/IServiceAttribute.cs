using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;

namespace WTA.Application.Abstractions;

public interface IServiceAttribute
{
    ServiceLifetime Lifetime { get; }
    Type ServiceType { get; }
    OSPlatform? Platform { get; }
}
