using System.ComponentModel.DataAnnotations;
using WTA.Application.Domain;

namespace WTA.Application.Identity.Domain;

[Display(Name = "权限",Order =3)]
public class Permission : BaseTreeEntity<Permission>
{
    public PermissionType Type { get; set; }
    public List<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
