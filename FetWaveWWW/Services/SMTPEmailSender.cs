using MeetWave.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

namespace MeetWave.Services
{
    public class SMTPEmailSender: IEmailSender, IExternalEmailSender
    {
        private readonly string server;
        private readonly int port;
        private bool useSSL;
        private readonly string fromEmail;
        private readonly string password;
        public SMTPEmailSender(IConfiguration configuration) 
        {
            fromEmail = configuration["Authentication:SMTP:Username"]!;
            server = configuration["Authentication:SMTP:Server"]!;
            port = int.Parse(configuration["Authentication:SMTP:Port"]!);
            useSSL = bool.Parse(configuration["Authentication:SMTP:UseSSL"]!);
            password = configuration["Authentication:SMTP:Password"]!;
        }

        private void SendMesssage(MimeMessage message)
        {
            using (var client = new SmtpClient())
            {
                client.Connect(server, port, MailKit.Security.SecureSocketOptions.StartTls);

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(fromEmail, password);

                client.Send(message);
                client.Disconnect(true);
            }
        }

            public async Task EmailListAsync(IEnumerable<string> emails, string subject, string body)
        {
            var mail = new MimeMessage();

            mail.From.Add(new MailboxAddress("MeetWave No Reply", fromEmail));
            mail.Bcc.AddRange(emails.Select(e => new MailboxAddress(e, e)));
            mail.Subject = subject;
            mail.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            try
            {
                SendMesssage(mail);
            }
            catch (Exception ex)
            {
                var a = ex;
            }
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var mail = new MimeMessage();

            mail.From.Add(new MailboxAddress("MeetWave No Reply", fromEmail));
            mail.To.Add(new MailboxAddress(email, email));
            mail.Subject = subject;
            mail.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

            try
            {
                SendMesssage(mail);
            }
            catch (Exception ex) 
            {
                var a = ex;
            }
        }
    }
}
