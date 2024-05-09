using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using TheBlogProject.Models;

namespace TheBlogProject.Services;

public class MailService(IOptions<AuthMessageSenderOptions> optionsAccessor, IConfiguration config)
    : IMailService
{
    private AuthMessageSenderOptions Options { get; } = optionsAccessor.Value; //set only via Secret Manager

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var client = new SendGridClient(Options.SendGridKey);
        var msg = new SendGridMessage()
        {
            From = new EmailAddress(config.GetSection("SendGrid").GetSection("SenderEmail").Value),
            Subject = subject,
            PlainTextContent = Regex.Replace(htmlMessage, "<[^>]*>", ""),
            HtmlContent = htmlMessage
        };
        msg.AddTo(new EmailAddress(email));
        msg.AddBcc("sean@seanschroder.com");
        msg.SetClickTracking(false,false);
        await client.SendEmailAsync(msg);    }

    public async Task SendContactEmailAsync(string email, string name, string subject, string htmlMessage)
    {
                var client = new SendGridClient(Options.SendGridKey);
                var msg = new SendGridMessage()
                {
                    From = new EmailAddress(config.GetSection("SendGrid").GetSection("SenderEmail").Value),
                    Subject = subject,
                    PlainTextContent = Regex.Replace(htmlMessage, "<[^>]*>", ""),
                    HtmlContent = htmlMessage
                };
                msg.AddTo(new EmailAddress(email));
                msg.AddBcc("sean@seanschroder.com");
                msg.SetClickTracking(false,false);
                await client.SendEmailAsync(msg);    
    }
}
