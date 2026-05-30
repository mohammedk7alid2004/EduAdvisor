using EduAdvisor.Domain.Entities.AuthModule;

namespace EduAdvisor.Application.Interfaces.Auth;

public interface ITokenService
{
    Task<TokenResult> GenerateJwtToken(User user);

    Task<RefreshToken> GenerateRefreshToken(User user);

    Task<RefreshToken?> GetRefreshTokenAsync(string token);

    Task RevokeRefreshTokenAsync(string token);
}

public class TokenResult
{
    public string Token { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
}