using System.Net.Mail;

namespace Warranty.Core.Services
{
    public interface ISendSmtpMailMessage
    {
        void SendMessage(MailMessage message);
    }
}
