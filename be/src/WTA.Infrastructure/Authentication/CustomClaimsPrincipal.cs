using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using WTA.Application.Abstractions;

namespace WTA.Infrastructure.Authentication;

public class CustomClaimsPrincipal : ClaimsPrincipal
{
    private readonly IServiceProvider _serviceProvider;

    public CustomClaimsPrincipal(IServiceProvider serviceProvider, ClaimsPrincipal claimsPrincipal) : base(claimsPrincipal)
    {
        _serviceProvider = serviceProvider;
    }

    public override bool IsInRole(string role)
    {
        using var scope = _serviceProvider.CreateScope();
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
        return permissionService.HasPermission(Identity!.Name!, role);
    }
}
