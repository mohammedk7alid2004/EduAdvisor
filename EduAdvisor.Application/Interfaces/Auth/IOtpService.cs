using EduAdvisor.Application.Common.Abstractions;

namespace EduAdvisor.Application.Interfaces.Auth;

public interface IOtpService
{
    Task<string> GenerateAndStoreAsync(
        string email,
        OtpType otpType,
        CancellationToken cancellationToken = default);

    Task<bool> ValidateAsync(
        string email,
        string otp,
        OtpType otpType,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(
        string email,
        OtpType otpType,
        CancellationToken cancellationToken = default);
}