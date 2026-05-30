using EduAdvisor.Domain.Entities.AuthModule;

namespace EduAdvisor.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendResetPasswordEmail(User user, string OTP);
        Task SendConfirmationEmail(User user, string OTP);
    }
}
