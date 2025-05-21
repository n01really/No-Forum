using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.UI.Services;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _config;
    public EmailSender(IConfiguration config) => _config = config;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var settings = _config.GetSection("EmailSettings");
        var smtpClient = new SmtpClient(settings["SmtpServer"])
        {
            Port = int.Parse(settings["SmtpPort"]),
            Credentials = new NetworkCredential(settings["SmtpUser"], settings["SmtpPass"]),
            EnableSsl = true,
        };
        var mail = new MailMessage(settings["SmtpUser"], email, subject, htmlMessage)
        {
            IsBodyHtml = true
        };
        await smtpClient.SendMailAsync(mail);
    }
}
