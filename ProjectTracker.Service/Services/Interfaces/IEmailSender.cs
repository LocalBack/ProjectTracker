using System.Threading.Tasks;
namespace ProjectTracker.Service.Services.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
