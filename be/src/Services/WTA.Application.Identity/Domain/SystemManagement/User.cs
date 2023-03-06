using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WTA.Application.Domain;

namespace WTA.Application.Identity.Domain.SystemManagement;

[Display(Name = "用户", Order = 1)]
[SystemManagement]
public class User : BaseEntity
{
    public int AccessFailedCount { get; set; }

    public string? Email { get; set; }

    public bool EmailConfirmed { get; set; }

    public bool LockoutEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public string? Name { get; set; }

    [ScaffoldColumn(false)]
    public string? NormalizedEmail { get; set; }

    [ScaffoldColumn(false)]
    public string? NormalizedUserName { get; set; }

    [ScaffoldColumn(false)]
    public string? PasswordHash { get; set; }

    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public string? RoleHash { get; set; }

    [ScaffoldColumn(false)]
    public string? SecurityStamp { get; set; }

    public bool TwoFactorEnabled { get; set; }

    [Display(Name = "用户名")]
    [ReadOnly(true)]
    public string? UserName { get; set; }

    public List<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
