namespace UndefinedCRM.Infrastructure.Security.Tokens.Google;

public class GoogleTokenInfo
{
    public string Sub { get; set; } = string.Empty; // ID único do usuário no Google
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Picture { get; set; } = string.Empty;
    public bool EmailVerified { get; set; }
    public string Locale { get; set; } = string.Empty;
}