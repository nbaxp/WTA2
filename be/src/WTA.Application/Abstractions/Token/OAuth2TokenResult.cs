namespace WTA.Application.Abstractions.Token;

public class OAuth2TokenResult
{
    public string TokenType = "Bearer";
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public long? expiresIn { get; set; }
}