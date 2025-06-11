using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.UI.Services;

// Implementerar e-posttj�nst enligt IEmailSender
public class EmailSender : IEmailSender
{
    private readonly IConfiguration _config; // H�ller konfiguration f�r e-postinst�llningar

    // Konstruktor som tar emot konfiguration via dependency injection
    public EmailSender(IConfiguration config) => _config = config;

    // Skickar ett e-postmeddelande asynkront
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // H�mtar e-postinst�llningar fr�n konfigurationen
        var settings = _config.GetSection("EmailSettings");

        // Skapar och konfigurerar SMTP-klient
        var smtpClient = new SmtpClient(settings["SmtpServer"])
        {
            Port = int.Parse(settings["SmtpPort"]),
            Credentials = new NetworkCredential(settings["SmtpUser"], settings["SmtpPass"]),
            EnableSsl = true,
        };

        // Skapar e-postmeddelande med HTML-inneh�ll
        var mail = new MailMessage(settings["SmtpUser"], email, subject, htmlMessage)
        {
            IsBodyHtml = true
        };

        // Skickar e-postmeddelandet asynkront
        await smtpClient.SendMailAsync(mail);
    }
}
