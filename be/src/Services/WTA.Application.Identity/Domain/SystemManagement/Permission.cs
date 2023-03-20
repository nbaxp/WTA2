using System.ComponentModel.DataAnnotations;
using WTA.Application.Abstractions.Data;
using WTA.Application.Domain;
using WTA.Application.Identity.Data;

namespace WTA.Application.Identity.Domain.SystemManagement;

[Display(Name = "权限", Order = 3)]
[SystemManagement]
[DbContext<IdentityDbContext>]
public class Permission : BaseTreeEntity<Permission>
{
    public PermissionType Type { get; set; }
    public List<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    public string? Columns { get; set; }
    public string? Path { get; set; }
    public string? Url { get; set; }
    public string? Component { get; set; }
    public string? Redirect { get; set; }
    public string? Icon { get; set; }
}
