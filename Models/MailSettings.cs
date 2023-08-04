namespace ContactHarbor.Models;

public class MailSettings
{
    public string? SmtpSenderName { get; set; }

    public string? SmtpEmail { get; set; }
    public string? SmtpApiKey { get; set; }

    public string? SmtpSecret { get; set; }

    public string? SmtpServer { get; set; }

    public int SmtpPort { get; set; }
}

