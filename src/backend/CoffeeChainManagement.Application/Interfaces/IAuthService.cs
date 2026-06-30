using CoffeeChainManagement.Application.DTOs.Auth;
using CoffeeChainManagement.Application.DTOs.Common;

namespace CoffeeChainManagement.Application.Interfaces;

// IAuthService dinh nghia use case dang nhap va lay profile user hien tai.
public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
    Task<AuthResponseDto?> RefreshAsync(RefreshRequestDto request, CancellationToken cancellationToken = default);
    Task<bool> LogoutAsync(RefreshRequestDto request, CancellationToken cancellationToken = default);
    Task<UserProfileDto?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request, CancellationToken cancellationToken = default);
    Task<bool> ResetPasswordAsync(Guid employeeId, ResetPasswordRequestDto request, CancellationToken cancellationToken = default);
    Task<PagedResultDto<UserSessionDto>> GetSessionsAsync(UserSessionQueryDto query, CancellationToken cancellationToken = default);
    Task<bool> RevokeSessionAsync(Guid sessionId, CancellationToken cancellationToken = default);
}
