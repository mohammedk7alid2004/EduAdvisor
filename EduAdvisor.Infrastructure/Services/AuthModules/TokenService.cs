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

public class TokenService : ITokenService
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

        var keyBytes = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
        _signingKey = new SymmetricSecurityKey(keyBytes);
    }

    public async Task<TokenResult> GenerateJwtToken(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var expiryMinutes = int.Parse(
            _configuration["Jwt:ExpiryMinutes"] ?? "60");

        var expiresAt = DateTime.Now.AddMinutes(expiryMinutes);

        var credentials = new SigningCredentials(
            _signingKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new TokenResult
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt
        };
    }

    public async Task<RefreshToken> GenerateRefreshToken(User user)
    {
        var expiryDays = int.Parse(
            _configuration["Jwt:RefreshTokenExpiryDays"] ?? "7");

        var refreshToken = new RefreshToken
        {
            Token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)),
            CreatedAt = DateTime.Now,
            ExpiresOn = DateTime.Now.AddDays(expiryDays),
            UserId = user.Id
        };

        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        return refreshToken;
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Token == token);
    }

    public async Task RevokeRefreshTokenAsync(string token)
    {
        await _context.RefreshTokens
            .Where(x => x.Token == token)
            .ExecuteUpdateAsync(x =>
                x.SetProperty(r => r.RevokedOn, DateTime.Now));
    }
}