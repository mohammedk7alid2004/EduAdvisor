using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EduAdvisor.Application.Interfaces.Auth;
using EduAdvisor.Domain.Entities.AuthModule;
using EduAdvisor.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EduAdvisor.Infrastructure.Services.AuthModules;

public sealed class TokenService : ITokenService
{
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly SymmetricSecurityKey _signingKey;

    public TokenService(
        UserManager<User> userManager,
        IConfiguration configuration,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _configuration = configuration;
        _context = context;

        var jwtKey = _configuration["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(jwtKey))
            throw new InvalidOperationException("JWT key is not configured.");

        var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
        _signingKey = new SymmetricSecurityKey(keyBytes);
    }

    public async Task<TokenResult> GenerateTokenAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        var jwtToken = await GenerateJwtTokenAsync(user, cancellationToken);

        var refreshToken = await GenerateRefreshTokenAsync(
            user,
            cancellationToken);

        var expiryMinutes = int.Parse(
            _configuration["Jwt:ExpiryMinutes"] ?? "60");

        return new TokenResult
        {
            Token = jwtToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes)
        };
    }

    public async Task<string> GenerateJwtTokenAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.FullName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var expiryMinutes = int.Parse(
            _configuration["Jwt:ExpiryMinutes"] ?? "60");

        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var credentials = new SigningCredentials(
            _signingKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<RefreshToken> GenerateRefreshTokenAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        var expiryDays = int.Parse(
            _configuration["Jwt:RefreshTokenExpiryDays"] ?? "7");

        var now = DateTime.UtcNow;

        var refreshToken = new RefreshToken
        {
            Token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)),
            CreatedAt = now,
            ExpiresOn = now.AddDays(expiryDays),
            UserId = user.Id
        };

        await _context.RefreshTokens.AddAsync(
            refreshToken,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return refreshToken;
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        var normalizedToken = token.Trim();

        return await _context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Token == normalizedToken,
                cancellationToken);
    }

    public async Task<RevokeRefreshTokenResult> RevokeRefreshTokenAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        var normalizedToken = token.Trim();

        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(
                x => x.Token == normalizedToken,
                cancellationToken);

        if (refreshToken is null)
            return RevokeRefreshTokenResult.NotFound;

        if (refreshToken.RevokedOn is not null)
            return RevokeRefreshTokenResult.AlreadyRevoked;

        if (refreshToken.ExpiresOn <= DateTime.UtcNow)
            return RevokeRefreshTokenResult.Expired;

        refreshToken.RevokedOn = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return RevokeRefreshTokenResult.Success;
    }
}