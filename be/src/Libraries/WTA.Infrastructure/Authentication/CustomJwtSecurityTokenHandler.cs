using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace WTA.Infrastructure.Authentication;

public class CustomJwtSecurityTokenHandler : JwtSecurityTokenHandler
{
    private readonly IServiceProvider _serviceProvider;

    public CustomJwtSecurityTokenHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
    {
        return new CustomClaimsPrincipal(_serviceProvider, base.ValidateToken(token, validationParameters, out validatedToken));
    }
}
