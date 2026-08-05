using System.Security.Cryptography;
using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.Common.Settings;
using EduAdvisor.Application.Interfaces.Auth;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using ServiceStack.Configuration;

namespace EduAdvisor.Infrastructure.Services.AuthModules;

public sealed class OtpService(
    IMemoryCache memoryCache,
    IHasherService hasherService,
    IOptions<OtpSettings> otpOptions)
    : IOtpService
{
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly IHasherService _hasherService = hasherService;
    private readonly OtpSettings _settings = otpOptions.Value;

    public Task<string> GenerateAndStoreAsync(
        string email,
        OtpType otpType,
        CancellationToken cancellationToken = default)
    {
        var otp = RandomNumberGenerator
            .GetInt32(100000, 1000000)
            .ToString();

        var cacheKey = BuildCacheKey(email, otpType);

        _memoryCache.Set(
            cacheKey,
            _hasherService.Hash(otp),
            TimeSpan.FromMinutes(_settings.ExpirationMinutes));

        return Task.FromResult(otp);
    }

    public Task<bool> ValidateAsync(
        string email,
        string otp,
        OtpType otpType,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = BuildCacheKey(email, otpType);

        if (!_memoryCache.TryGetValue<string>(cacheKey, out var hashedOtp))
            return Task.FromResult(false);

        var isValid = _hasherService.Verify(
            otp,
            hashedOtp!);

        if (isValid)
        {
            _memoryCache.Remove(cacheKey);
        }

        return Task.FromResult(isValid);
    }

    public Task RemoveAsync(
        string email,
        OtpType otpType,
        CancellationToken cancellationToken = default)
    {
        _memoryCache.Remove(
            BuildCacheKey(email, otpType));

        return Task.CompletedTask;
    }

    private static string BuildCacheKey(
        string email,
        OtpType otpType)
    {
        return $"{otpType}_{email.Trim().ToLowerInvariant()}";
    }
}