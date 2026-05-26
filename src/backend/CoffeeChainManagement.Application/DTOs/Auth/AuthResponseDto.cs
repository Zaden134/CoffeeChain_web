namespace CoffeeChainManagement.Application.DTOs.Auth;

// AuthResponseDto tra token va thong tin user sau khi login thanh cong.
public sealed record AuthResponseDto(
    string AccessToken,
    DateTime ExpiresAtUtc,
    UserProfileDto User);
