using System.ComponentModel.DataAnnotations;
using WTA.Application.Domain;

namespace WTA.Application.Identity.Domain.SystemManagement;

[Display(Name = "角色权限")]
[SystemManagement]
public class RolePermission : BaseEntity, IAssociation
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public Role Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}
