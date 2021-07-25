using CoreLogicLib.Comm;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace WobigTools.Data
{
    public class EmailService : IEmailSender
    {
        Task IEmailSender.SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Communication.SendEmail(subject, htmlMessage, new string[] { email }, true);
            return Task.CompletedTask;
        }
    }
}
