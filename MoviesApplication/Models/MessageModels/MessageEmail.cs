using MimeKit;

namespace MoviesApplication.Models.MessageModels
{
    public class MessageEmail
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public IFormFileCollection Attachments { get; set; }

        public MessageEmail(IEnumerable<string> to, string subject, string content, IFormFileCollection attachments) 
        {
            To = new List<MailboxAddress>();

            To.AddRange(to.Select(x => new MailboxAddress(x, x)));
            Subject = subject;
            Content = content;
            Attachments = attachments;
        }
    }
}
