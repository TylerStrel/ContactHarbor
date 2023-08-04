using ContactHarbor.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;


namespace ContactHarbor.Services;

public class EmailService : IEmailSender
{
    private readonly MailSettings _mailSettings;

    public EmailService(IOptions<MailSettings> mailSettings)
    {
        _mailSettings = mailSettings.Value;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var senderName = _mailSettings.SmtpSenderName ?? Environment.GetEnvironmentVariable("SmtpSenderName");
        var senderEmail = _mailSettings.SmtpEmail ?? Environment.GetEnvironmentVariable("SmtpEmail");
        var apiKey = _mailSettings.SmtpApiKey ?? Environment.GetEnvironmentVariable("SmtpApiKey");
        var host = _mailSettings.SmtpServer ?? Environment.GetEnvironmentVariable("SmtpServer");
        var port = _mailSettings.SmtpPort != 0 ? _mailSettings.SmtpPort : int.Parse(Environment.GetEnvironmentVariable("SmtpPort")!);
        var secret = _mailSettings.SmtpSecret ?? Environment.GetEnvironmentVariable("SmtpSecret");


        try
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse($"{senderName} <{senderEmail}>"));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlMessage };

            using var client = new SmtpClient();
            await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(apiKey, secret);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

        }
        catch
        {
            throw;
        }
    }
}
