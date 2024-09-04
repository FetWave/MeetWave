using Microsoft.AspNetCore.Identity.UI.Services;

namespace MeetWave.Services.Interfaces
{
    public interface IExternalEmailSender : IEmailSender
    {
        Task EmailListAsync(IEnumerable<string> emails, string subject, string body);
    }
}
