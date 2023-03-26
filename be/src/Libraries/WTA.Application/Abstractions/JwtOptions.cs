namespace WTA.Application.Abstractions;

public class JwtOptions
{
    public const string Position = "Identity";
    public string Issuer { get; set; } = "value";
    public string Audience { get; set; } = "value";
    public string Key { get; set; } = "1234567890abcdef";
    public TimeSpan AccessTokenExpires { get; set; } = TimeSpan.FromMinutes(10);
    public TimeSpan RefreshTokenExpires { get; set; } = TimeSpan.FromDays(14);
}
