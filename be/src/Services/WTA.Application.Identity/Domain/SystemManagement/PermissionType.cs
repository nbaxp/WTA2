using System.ComponentModel.DataAnnotations;

namespace WTA.Application.Identity.Domain.SystemManagement;

public enum PermissionType
{
    [Display(Name = "分组")]
    Group,

    [Display(Name = "资源")]
    Resource,

    [Display(Name = "权限")]
    Permission
}
