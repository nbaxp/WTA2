using System.ComponentModel.DataAnnotations;
using WTA.Application.Domain;

namespace WTA.Application.Identity.Domain;

[Display(Name = "角色")]
public class Role : BaseEntity
{
    [Display(Name = "编号")]
    public string Nummber { get; set; } = null!;

    [Display(Name = "名称")]
    public string Name { get; set; } = null!;

    public string? PermissionHash { get; set; }
    public List<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public List<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
