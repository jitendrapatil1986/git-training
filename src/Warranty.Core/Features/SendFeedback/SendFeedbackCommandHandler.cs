namespace Warranty.Core.Features.SendFeedback
{
    using Services;

    public class SendFeedbackCommandHandler : ICommandHandler<SendFeedbackCommand>
    {
        private readonly ISendSmtpMailMessage _sendSmtpMailMessage;

        public SendFeedbackCommandHandler(ISendSmtpMailMessage sendSmtpMailMessage)
        {
            _sendSmtpMailMessage = sendSmtpMailMessage;
        }

        public void Handle(SendFeedbackCommand message)
        {
            _sendSmtpMailMessage.SendMessage(message.MailMessage);
        }
    }
}
