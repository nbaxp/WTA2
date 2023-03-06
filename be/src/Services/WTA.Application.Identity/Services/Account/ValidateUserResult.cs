using WTA.Application.Abstractions.Token;
using WTA.Application.Identity.Domain.SystemManagement;

namespace WTA.Application.Identity.Services.Account;

public class ValidateUserResult
{
    public ValidateUserStatus Status { get; set; }
    public OAuth2TokenResult? TokenResult { get; set; }
    public User? User { get; set; }
}
