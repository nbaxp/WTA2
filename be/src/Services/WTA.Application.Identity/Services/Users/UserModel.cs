using System.ComponentModel.DataAnnotations;
using WTA.Application.Application;
using WTA.Application.Identity.Domain;

namespace WTA.Application.Identity.Services.Users;

[Model<User>, SearchModel<User>, ListModel<User>]
public class UserModel
{
    [OperatorType(OperatorType.Contains)]
    [Display]
    public string? UserName { get; set; }

    [OperatorType(OperatorType.Equal)]
    [Display]
    public string? Email { get; set; }

    [Display]
    public bool EmailConfirmed { get; set; }
}
