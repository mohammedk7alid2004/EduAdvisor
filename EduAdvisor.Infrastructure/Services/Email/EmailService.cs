using EduAdvisor.Application.Interfaces;
using EduAdvisor.Domain.Entities.AuthModule;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EduAdvisor.Infrastructure.Services.Email;

public class EmailService(
    IOptions<MailSettings> mailSettings,
    ILogger<EmailService> logger)
    : IEmailSender, IEmailService
{
    private readonly MailSettings _mailSettings = mailSettings.Value;
    private readonly ILogger<EmailService> _logger = logger;

    public async Task SendEmailAsync(
        string email,
        string subject,
        string htmlMessage)
    {
        var message = new MimeMessage
        {
            Sender = MailboxAddress.Parse(_mailSettings.Mail),
            Subject = subject
        };

        message.To.Add(MailboxAddress.Parse(email));

        var builder = new BodyBuilder
        {
            HtmlBody = htmlMessage
        };

        message.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();

        _logger.LogInformation("Sending email to {Email}", email);

        smtp.Connect(
            _mailSettings.Host,
            _mailSettings.Port,
            SecureSocketOptions.StartTls);

        smtp.Authenticate(
            _mailSettings.Mail,
            _mailSettings.Password);

        await smtp.SendAsync(message);
        smtp.Disconnect(true);
    }

    public async Task SendResetPasswordEmail(User user, string otp)
    {
        var emailBody = EmailBodyBuilder.GenerateEmailBody(
            "ForgetPassword",
            new Dictionary<string, string>
            {
                { "{{FullName}}", user.FullName },
                { "{{OtpCode}}", otp }
            });

        await SendEmailAsync(
            user.Email!,
            "✅ EduAdvisor: Reset Password",
            emailBody);
    }

    public async Task SendConfirmationEmail(User user, string otp)
    {
        var emailBody = EmailBodyBuilder.GenerateEmailBody(
            "EmailConfirmation",
            new Dictionary<string, string>
            {
                { "{{FullName}}", user.FullName },
                { "{{OtpCode}}", otp }
            });

        await SendEmailAsync(
            user.Email!,
            "✅ EduAdvisor: Email Confirmation",
            emailBody);
    }
}