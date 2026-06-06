using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using RailwayManagementSystemAPI.Configuration;

namespace RailwayManagementSystemAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendDailyReportAsync(string date, string? attachmentPath)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));

            foreach (var recipient in _emailSettings.Recipients)
                message.To.Add(MailboxAddress.Parse(recipient));

            message.Subject = $"Railway Daily Report - {date}";

            var bodyBuilder = new BodyBuilder();

            if (attachmentPath != null && File.Exists(attachmentPath))
            {
                bodyBuilder.HtmlBody = $@"
                    <h2>Railway Management System</h2>
                    <p>Please find attached the daily operations report for <strong>{date}</strong>.</p>
                    <p>The report includes:</p>
                    <ul>
                        <li>Delays by route</li>
                        <li>Delays by station</li>
                        <li>Delays by train</li>
                        <li>On-time performance</li>
                        <li>Delays by type</li>
                    </ul>
                ";
                bodyBuilder.Attachments.Add(attachmentPath);
                _logger.LogInformation("Attaching report from {Path}", attachmentPath);
            }
            else
            {
                bodyBuilder.HtmlBody = $@"
                    <h2>Railway Management System</h2>
                    <p>No completed trips were found for <strong>{date}</strong>.</p>
                    <p>No report was generated for this date.</p>
                ";
                _logger.LogInformation("No report found for {Date} — sending no data email", date);
            }

            message.Body = bodyBuilder.ToMessageBody();

            using var clinet = new SmtpClient();
            await clinet.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await clinet.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.SenderPassword);
            await clinet.SendAsync(message);
            await clinet.DisconnectAsync(true);

            _logger.LogInformation("Daily report email sent for {Date} to {Recipients}",
                date, string.Join(", ", _emailSettings.Recipients));
        }
    }
}