using System.Security.Claims;

namespace WTA.Application.Abstractions.Token;

public interface ITokenService
{
    OAuth2TokenResult CreateAuth2TokenResult(string userName, bool rememberMe, params Claim[] additionalClaims);

    string CreatAccessTokenForCookie(string userName, bool rememberMe, out TimeSpan timeout, params Claim[] additionalClaims);
}
