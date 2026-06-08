namespace CoffeeChainManagement.Application.DTOs.Auth;

// AuthResponseDto tra token va thong tin user sau khi login thanh cong.
public sealed record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAtUtc,
    DateTime RefreshTokenExpiresAtUtc,
    UserProfileDto User);
