using MoviesApplication.Models.MessageModels;

namespace MoviesApplication.Services.EmailServices
{
    public interface IEmailSenderService
    {
        void SendEmail(MessageEmail message);
        Task SendEmailAsync(MessageEmail message);
    }
}