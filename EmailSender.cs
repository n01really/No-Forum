using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.UI.Services;

// Implementerar e-posttjänst enligt IEmailSender
public class EmailSender : IEmailSender
{
    private readonly IConfiguration _config; // Håller konfiguration för e-postinställningar

    // Konstruktor som tar emot konfiguration via dependency injection
    public EmailSender(IConfiguration config) => _config = config;

    // Skickar ett e-postmeddelande asynkront
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Hämtar e-postinställningar från konfigurationen
        var settings = _config.GetSection("EmailSettings");

        // Skapar och konfigurerar SMTP-klient
        var smtpClient = new SmtpClient(settings["SmtpServer"])
        {
            Port = int.Parse(settings["SmtpPort"]),
            Credentials = new NetworkCredential(settings["SmtpUser"], settings["SmtpPass"]),
            EnableSsl = true,
        };

        // Skapar e-postmeddelande med HTML-innehåll
        var mail = new MailMessage(settings["SmtpUser"], email, subject, htmlMessage)
        {
            IsBodyHtml = true
        };

        // Skickar e-postmeddelandet asynkront
        await smtpClient.SendMailAsync(mail);
    }
}
