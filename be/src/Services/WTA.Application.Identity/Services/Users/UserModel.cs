using WTA.Application.Abstractions.Controllers;
using WTA.Application.Application;
using WTA.Application.Identity.Domain;

namespace WTA.Application.Identity.Services.Users;

[Model<User>, ListModel<User>, SearchModel<User>]
public class UserModel : PaginationModel<UserModel>
{
    [OperatorType(OperatorType.Contains)]
    public string? UserName { get; set; }

    public string? NormalizedUserName { get; set; }

    [OperatorType(OperatorType.Equal)]
    public string? Email { get; set; }

    public string? NormalizedEmail { get; set; }
    public bool EmailConfirmed { get; set; }
}
