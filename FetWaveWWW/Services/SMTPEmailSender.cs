using MeetWave.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;

namespace MeetWave.Services
{
    public class SMTPEmailSender: IEmailSender, IExternalEmailSender
    {
        private readonly SmtpClient client;
        public SMTPEmailSender(IConfiguration configuration) 
        {

            client = new SmtpClient(configuration["Authentication:SMTP:Server"], int.Parse(configuration["Authentication:SMTP:Port"]!));

            client.Credentials = new System.Net.NetworkCredential(configuration["Authentication:SMTP:Username"], configuration["Authentication:SMTP:Password"]);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
        }

        public Task EmailListAsync(IEnumerable<string> emails, string subject, string body)
        {
            throw new NotImplementedException();
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            throw new NotImplementedException();
        }
    }
}
