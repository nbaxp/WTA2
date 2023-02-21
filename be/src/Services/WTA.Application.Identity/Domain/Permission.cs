using System.ComponentModel.DataAnnotations;
using WTA.Application.Domain;

namespace WTA.Application.Identity.Domain;

[Display(Name = "权限")]
public class Permission : TreeEntity<Permission>
{
    public PermissionType Type { get; set; }
    public List<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
