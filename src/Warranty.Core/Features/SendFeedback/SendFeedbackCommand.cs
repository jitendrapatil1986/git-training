namespace Warranty.Core.Features.SendFeedback
{
    using System.Net.Mail;

    public class SendFeedbackCommand : ICommand
    {
        public MailMessage MailMessage { get; set; }
    }
}
