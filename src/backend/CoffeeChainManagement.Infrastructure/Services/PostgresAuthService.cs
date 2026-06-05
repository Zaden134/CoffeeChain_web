using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CoffeeChainManagement.Application.DTOs.Auth;
using CoffeeChainManagement.Application.Interfaces;
using CoffeeChainManagement.Domain.Entities;
using CoffeeChainManagement.Infrastructure.Auth;
using CoffeeChainManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace CoffeeChainManagement.Infrastructure.Services;

// PostgresAuthService xac thuc user bang PostgreSQL va phat JWT cho frontend.
internal sealed class PostgresAuthService(
    CoffeeChainDbContext dbContext,
    IPasswordHasher<Employee> passwordHasher,
    IAuditLogService auditLogService,
    IOptions<JwtOptions> jwtOptions) : IAuthService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var normalizedUsername = request.Username.Trim().ToLowerInvariant();

        var user = await dbContext.Employees
            .AsNoTracking()
            .SingleOrDefaultAsync(
                employee => employee.IsActive && employee.Username.ToLower() == normalizedUsername,
                cancellationToken);

        if (user is null)
        {
            await auditLogService.WriteAsync(
                "AUTH_LOGIN",
                nameof(Employee),
                $"Invalid credentials for {request.Username.Trim()}",
                false,
                username: request.Username.Trim(),
                cancellationToken: cancellationToken);
            return null;
        }

        var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            await auditLogService.WriteAsync(
                "AUTH_LOGIN",
                nameof(Employee),
                $"Invalid credentials for {user.Username}",
                false,
                user.Id,
                user.Username,
                user.BranchId,
                cancellationToken: cancellationToken);
            return null;
        }

        var response = await CreateAuthResponseAsync(user, cancellationToken);
        await auditLogService.WriteAsync(
            "AUTH_LOGIN",
            nameof(Employee),
            $"Login success for {user.Username}",
            true,
            user.Id,
            user.Username,
            user.BranchId,
            cancellationToken: cancellationToken);

        return response;
    }

    public async Task<AuthResponseDto?> RefreshAsync(RefreshRequestDto request, CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(request.RefreshToken);
        var session = await dbContext.RefreshTokenSessions
            .SingleOrDefaultAsync(item => item.TokenHash == tokenHash, cancellationToken);

        if (session is null || session.RevokedAtUtc is not null || session.ExpiresAtUtc <= DateTime.UtcNow)
        {
            await auditLogService.WriteAsync(
                "AUTH_REFRESH",
                nameof(RefreshTokenSession),
                "Refresh token rejected",
                false,
                cancellationToken: cancellationToken);
            return null;
        }

        var user = await dbContext.Employees.SingleOrDefaultAsync(employee => employee.Id == session.EmployeeId && employee.IsActive, cancellationToken);
        if (user is null)
        {
            await auditLogService.WriteAsync(
                "AUTH_REFRESH",
                nameof(Employee),
                "Refresh token target user is inactive or missing",
                false,
                session.EmployeeId,
                cancellationToken: cancellationToken);
            return null;
        }

        var response = await RotateSessionAsync(user, session, cancellationToken);
        await auditLogService.WriteAsync(
            "AUTH_REFRESH",
            nameof(Employee),
            $"Refresh success for {user.Username}",
            true,
            user.Id,
            user.Username,
            user.BranchId,
            cancellationToken: cancellationToken);

        return response;
    }

    public async Task<bool> LogoutAsync(RefreshRequestDto request, CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(request.RefreshToken);
        var session = await dbContext.RefreshTokenSessions.SingleOrDefaultAsync(item => item.TokenHash == tokenHash, cancellationToken);

        if (session is null)
        {
            await auditLogService.WriteAsync(
                "AUTH_LOGOUT",
                nameof(RefreshTokenSession),
                "Logout requested for unknown refresh token",
                false,
                cancellationToken: cancellationToken);
            return false;
        }

        session.RevokedAtUtc = DateTime.UtcNow;
        session.RevokedByIp = GetIpAddress();
        session.UpdatedAtUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        var user = await dbContext.Employees.AsNoTracking().SingleOrDefaultAsync(employee => employee.Id == session.EmployeeId, cancellationToken);
        await auditLogService.WriteAsync(
            "AUTH_LOGOUT",
            nameof(Employee),
            "Logout success",
            true,
            session.EmployeeId,
            user?.Username,
            user?.BranchId,
            cancellationToken: cancellationToken);

        return true;
    }

    public async Task<UserProfileDto?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Employees
            .AsNoTracking()
            .SingleOrDefaultAsync(employee => employee.Id == userId && employee.IsActive, cancellationToken);

        return user is null ? null : MapUser(user);
    }

    private async Task<AuthResponseDto> CreateAuthResponseAsync(Employee user, CancellationToken cancellationToken)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes);
        var refreshToken = GenerateToken();
        var refreshExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtOptions.RefreshExpiryDays);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        if (user.BranchId.HasValue)
        {
            claims.Add(new Claim("branch_id", user.BranchId.Value.ToString()));
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        dbContext.RefreshTokenSessions.Add(new RefreshTokenSession
        {
            EmployeeId = user.Id,
            TokenHash = HashToken(refreshToken),
            ExpiresAtUtc = refreshExpiresAtUtc,
            CreatedByIp = GetIpAddress()
        });
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto(
            new JwtSecurityTokenHandler().WriteToken(token),
            refreshToken,
            expiresAtUtc,
            refreshExpiresAtUtc,
            MapUser(user));
    }

    private async Task<AuthResponseDto> RotateSessionAsync(Employee user, RefreshTokenSession session, CancellationToken cancellationToken)
    {
        session.RevokedAtUtc = DateTime.UtcNow;
        session.RevokedByIp = GetIpAddress();

        var refreshToken = GenerateToken();
        var refreshExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtOptions.RefreshExpiryDays);

        dbContext.RefreshTokenSessions.Add(new RefreshTokenSession
        {
            EmployeeId = user.Id,
            TokenHash = HashToken(refreshToken),
            ExpiresAtUtc = refreshExpiresAtUtc,
            CreatedByIp = GetIpAddress()
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        if (user.BranchId.HasValue)
        {
            claims.Add(new Claim("branch_id", user.BranchId.Value.ToString()));
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new AuthResponseDto(
            new JwtSecurityTokenHandler().WriteToken(token),
            refreshToken,
            expiresAtUtc,
            refreshExpiresAtUtc,
            MapUser(user));
    }

    private static string GenerateToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }

    private string? GetIpAddress()
        => null;

    private static UserProfileDto MapUser(Employee user)
        => new(
            user.Id,
            user.Username,
            user.FullName,
            user.Email,
            user.Role.ToString(),
            user.BranchId);
}
