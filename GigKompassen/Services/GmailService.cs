using GigKompassen.Models.Accounts;
using GigKompassen.Settings;

using MailKit.Net.Smtp;
using MailKit.Security;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MimeKit;

namespace GigKompassen.Services
{
  public class GmailService : IEmailSender<ApplicationUser>
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
      message.From.Add(new MailboxAddress(Options.SenderName, Options.Email));
      message.To.Add(new MailboxAddress("", email));
      message.Subject = subject;
      BodyBuilder bodyBuilder = new BodyBuilder();
      bodyBuilder.HtmlBody = htmlMessage;
      message.Body = bodyBuilder.ToMessageBody();
      Logger.LogInformation($"Sending email to {email}");
      using (var client = new SmtpClient())
      {
        await client.ConnectAsync(_host, _port, SecureSocketOptions.StartTls);
        Logger.LogInformation($"Connected to SMTP server {_host} as {Options.SenderName}");
        await client.AuthenticateAsync(Options.Email, Options.Password);
        Logger.LogInformation($"Authenticated as {Options.Email}");
        await client.SendAsync(message);
        Logger.LogInformation($"Sent email to {email}");
        await client.DisconnectAsync(true);
        Logger.LogInformation($"Disconnected from SMTP server {_host} as {Options.SenderName}");
      }
    }

    public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
      await SendEmailAsync(email, "Confirm your email", $"Please confirm your email by clicking this link: <a href='{confirmationLink}'>link</a>");
    }

    public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
      await SendEmailAsync(email, "Reset your password", $"Please reset your password by clicking this link: <a href='{resetLink}'>link</a>");
    }

    public async Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
      await SendEmailAsync(email, "Reset your password", $"Your password reset code is: {resetCode}");
    }
  }
}
