using GigKompassen.Settings;

using MailKit.Net.Smtp;
using MailKit.Security;

using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MimeKit;

namespace GigKompassen.Services
{
  public class GmailService : IEmailSender
  {

    public EmailSettings Options;
    public ILogger<GmailService> Logger;

    private const string _host = "smtp.gmail.com";
    private const int _port = 587;


    public GmailService(IOptions<EmailSettings> mailOptions, ILogger<GmailService> logger) 
    {
      Options = mailOptions.Value;
      Logger = logger;
    }
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
      MimeMessage message = new MimeMessage();
      message.From.Add(new MailboxAddress(Options.Username, Options.Email));
      message.To.Add(new MailboxAddress("", email));
      message.Subject = subject;
      BodyBuilder bodyBuilder = new BodyBuilder();
      bodyBuilder.HtmlBody = htmlMessage;
      message.Body = bodyBuilder.ToMessageBody();
      Logger.LogInformation($"Sending email to {email}");
      using (var client = new SmtpClient())
      {
        await client.ConnectAsync(_host, _port, SecureSocketOptions.StartTls);
        Logger.LogInformation($"Connected to SMTP server {_host} as {Options.Username}");
        await client.AuthenticateAsync(Options.Email, Options.Password);
        Logger.LogInformation($"Authenticated as {Options.Email}");
        await client.SendAsync(message);
        Logger.LogInformation($"Sent email to {email}");
        await client.DisconnectAsync(true);
        Logger.LogInformation($"Disconnected from SMTP server {_host} as {Options.Username}");
      }
    }

  }
}
