using EduAdvisor.Domain.Entities.AuthModule;

namespace EduAdvisor.Application.Interfaces.Auth;

public interface ITokenService
{
    Task<TokenResult> GenerateTokenAsync(
        User user,
        CancellationToken cancellationToken = default);

    Task<string> GenerateJwtTokenAsync(
        User user,
        CancellationToken cancellationToken = default);

    Task<RefreshToken> GenerateRefreshTokenAsync(
        User user,
        CancellationToken cancellationToken = default);

    Task<RefreshToken?> GetRefreshTokenAsync(
        string token,
        CancellationToken cancellationToken = default);

    Task<RevokeRefreshTokenResult> RevokeRefreshTokenAsync(
        string token,
        CancellationToken cancellationToken = default);
}

public sealed class TokenResult
{
    public string Token { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
}

public enum RevokeRefreshTokenResult
{
    Success = 1,
    NotFound = 2,
    AlreadyRevoked = 3,
    Expired = 4
}