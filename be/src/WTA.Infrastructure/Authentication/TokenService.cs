using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WTA.Application.Abstractions.Token;
using WTA.Application.Abstractions;

namespace WTA.Infrastructure.Authentication;

[Service<ITokenService>]
public class TokenService : ITokenService
{
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly JwtOptions _jwtOptions;
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
    private readonly SigningCredentials _credentials;

    public TokenService(TokenValidationParameters tokenValidationParameters,
        JwtSecurityTokenHandler jwtSecurityTokenHandler,
        SigningCredentials credentials,
        IOptions<JwtOptions> jwtOptions)
    {
        _tokenValidationParameters = tokenValidationParameters;
        _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
        _credentials = credentials;
        _jwtOptions = jwtOptions.Value;
    }

    public string CreatAccessTokenForCookie(string userName, bool rememberMe, out TimeSpan timeout, params Claim[] additionalClaims)
    {
        var now = DateTime.UtcNow;
        timeout = rememberMe ? TimeSpan.FromDays(365) : _jwtOptions.RefreshTokenExpires;
        var subject = CreateSubject(userName, additionalClaims);
        var toeken = CreateToken(subject, now, timeout);
        return toeken;
    }

    public OAuth2TokenResult CreateAuth2TokenResult(string userName, bool rememberMe, params Claim[] additionalClaims)
    {
        var now = DateTime.UtcNow;
        var subject = CreateSubject(userName, additionalClaims);
        return new OAuth2TokenResult
        {
            AccessToken = CreateToken(subject, now, _jwtOptions.AccessTokenExpires),
            RefreshToken = CreateToken(subject, now, rememberMe ? TimeSpan.FromDays(365) : _jwtOptions.RefreshTokenExpires),
            expiresIn = (long)_jwtOptions.AccessTokenExpires.TotalSeconds
        };
    }

    private ClaimsIdentity CreateSubject(string userName, Claim[] additionalClaims)
    {
        var claims = new List<Claim>(additionalClaims){
        new Claim(_tokenValidationParameters.NameClaimType,userName)
    };
        var subject = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
        return subject;
    }

    private string CreateToken(ClaimsIdentity subject, DateTime now, TimeSpan timeout)
    {
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            SigningCredentials = _credentials,
            Subject = subject,
            IssuedAt = now,
            NotBefore = now,
            Expires = now.Add(timeout),
        };
        var securityToken = _jwtSecurityTokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        var token = _jwtSecurityTokenHandler.WriteToken(securityToken);
        return token;
    }
}
