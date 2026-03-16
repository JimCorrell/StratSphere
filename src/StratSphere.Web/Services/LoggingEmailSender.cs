using Microsoft.AspNetCore.Identity;
using StratSphere.Core.Entities;

namespace StratSphere.Web.Services;

/// <summary>
/// Dev-mode email sender: logs confirmation/reset links to the console instead of sending real email.
/// Replace with a real IEmailSender implementation (SendGrid, SMTP, etc.) before production.
/// </summary>
public class LoggingEmailSender(ILogger<LoggingEmailSender> logger) : IEmailSender<ApplicationUser>
{
    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        logger.LogWarning(
            "[DEV EMAIL] Confirmation link for {Email}: {Link}",
            email, confirmationLink);
        return Task.CompletedTask;
    }

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        logger.LogWarning(
            "[DEV EMAIL] Password reset link for {Email}: {Link}",
            email, resetLink);
        return Task.CompletedTask;
    }

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        logger.LogWarning(
            "[DEV EMAIL] Password reset code for {Email}: {Code}",
            email, resetCode);
        return Task.CompletedTask;
    }
}
