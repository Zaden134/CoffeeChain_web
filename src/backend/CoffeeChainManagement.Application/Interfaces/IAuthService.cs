using CoffeeChainManagement.Application.DTOs.Auth;

namespace CoffeeChainManagement.Application.Interfaces;

// IAuthService dinh nghia use case dang nhap va lay profile user hien tai.
public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
    Task<UserProfileDto?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
