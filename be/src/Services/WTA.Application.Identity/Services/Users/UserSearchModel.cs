using System.ComponentModel.DataAnnotations;
using WTA.Application.Abstractions.Controllers;

namespace WTA.Application.Identity.Services.Users;

public class UserSearchModel : PaginationModel<UserModel>
{
    [Display]
    public string? UserName { get; set; }
}
