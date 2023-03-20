using System.ComponentModel.DataAnnotations;
using WTA.Application.Abstractions.Data;
using WTA.Application.Domain;
using WTA.Application.Identity.Data;

namespace WTA.Application.Identity.Domain.SystemManagement;

[Display(Name = "角色权限")]
[SystemManagement]
[DbContext<IdentityDbContext>]
public class RolePermission : BaseEntity, IAssociation
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public string? RowLimit { get; set; }
    public string? ColumnLimit { get; set; }
    public Role Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}
