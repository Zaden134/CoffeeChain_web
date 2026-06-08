namespace CoffeeChainManagement.Infrastructure.Auth;

// JwtOptions gom cac gia tri ky token de login va authorize thong nhat.
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required string SigningKey { get; set; }
    public int ExpiryMinutes { get; set; } = 120;
    public int RefreshExpiryDays { get; set; } = 7;
}
