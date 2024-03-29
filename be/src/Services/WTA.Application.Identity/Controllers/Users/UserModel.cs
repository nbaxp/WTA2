using System.ComponentModel.DataAnnotations;
using WTA.Application.Application;
using WTA.Application.Identity.Domain.SystemManagement;

namespace WTA.Application.Identity.Controllers.Users;

[Model<User>, SearchModel<User>, ListModel<User>]
public class UserModel
{
    [OperatorType(OperatorType.Contains)]
    [Display]
    public string? UserName { get; set; }

    [Display]
    public string? Avatar { get; set; }

    [OperatorType(OperatorType.Equal)]
    [Display]
    public string? Email { get; set; }

    [Display]
    public bool EmailConfirmed { get; set; }
}
