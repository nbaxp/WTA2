using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WTA.Application.Domain;

namespace WTA.Application.Identity.Domain.SystemManagement;

[Display(Name = "角色", Order = 2)]
[SystemManagement]
public class Role : BaseEntity
{
    [Display(Name = "编号")]
    public string Nummber { get; set; } = null!;

    [Display(Name = "名称")]
    public string Name { get; set; } = null!;

    [HiddenInput]
    public string? PermissionHash { get; set; }
    [HiddenInput]
    public List<UserRole> UserRoles { get; set; } = new List<UserRole>();
    [HiddenInput]
    public List<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
