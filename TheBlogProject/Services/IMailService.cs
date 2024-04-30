using Microsoft.AspNetCore.Identity.UI.Services;

namespace TheBlogProject.Services;

public interface IMailService : IEmailSender
{
    new Task SendEmailAsync(string email, string subject, string htmlMessage);
}