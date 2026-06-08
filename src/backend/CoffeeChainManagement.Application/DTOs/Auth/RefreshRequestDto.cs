namespace CoffeeChainManagement.Application.DTOs.Auth;

// RefreshRequestDto nhan refresh token tu frontend de cap lai access token.
public sealed record RefreshRequestDto(string RefreshToken);
