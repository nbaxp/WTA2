using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using WTA.Application.Abstractions;
using WTA.Application.Abstractions.Controllers;
using WTA.Application.Extensions;
using WTA.Application.Identity.Domain.SystemManagement;

namespace WTA.Application.Identity.Controllers.Permissions;

[ApiExplorerSettings(GroupName = nameof(Identity))]
[Service<IPermissionService>, Service<IMenuService>]
public class PermissionController : GenericController<Permission, Permission, Permission, Permission>, IPermissionService, IMenuService
{
    public const string CACHE_KEY = nameof(Permission);
    private readonly IDistributedCache _distributedCache;
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IRepository<User> _userRepository;

    public PermissionController(IRepository<User> userRepository, IRepository<Permission> permissionRepository, IDistributedCache distributedCache) : base(permissionRepository)
    {
        this._userRepository = userRepository;
        this._permissionRepository = permissionRepository;
        this._distributedCache = distributedCache;
    }

    [HttpPost]
    public List<MenuItemModel> GetPermissions()
    {
        return this._permissionRepository
                    .AsNoTracking()
                    .OrderBy(o => o.DisplayOrder)
                    .ToList<Permission, MenuItemModel>();
    }

    [HttpPost]
    public bool HasPermission(string userName, string permissionNumber)
    {
        return this._userRepository.AsNoTracking()
          .Where(o => o.UserName == userName)
          .Any(o => o.UserRoles.Any(o => o.Role.RolePermissions.Any(rp => rp.Permission.Number == permissionNumber)));
    }
}
