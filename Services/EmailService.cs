using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;


namespace ContactHarbor.Services;

public class EmailService : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var emailAddress = _configuration["ElasticEmail:SmtpUsername"];
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(emailAddress));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlMessage };

        using var client = new SmtpClient();
        await client.ConnectAsync(_configuration["ElasticEmail:SmtpServer"], int.Parse(_configuration["ElasticEmail:SmtpPort"]!), SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(emailAddress, _configuration["ElasticEmail:SmtpApiKey"]);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
