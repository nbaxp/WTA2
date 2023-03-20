using System.ComponentModel.DataAnnotations;
using WTA.Application.Abstractions.Data;
using WTA.Application.Domain;
using WTA.Application.Identity.Data;

namespace WTA.Application.Identity.Domain.SystemManagement;

[Display(Name = "用户角色")]
[SystemManagement]
[DbContext<IdentityDbContext>]
public class UserRole : BaseEntity, IAssociation
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
