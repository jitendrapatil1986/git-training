namespace Warranty.Core.Services
{
    using System.Net.Mail;

    public class SendSmtpMailMessage : ISendSmtpMailMessage
    {
        public void SendMessage(MailMessage message)
        {
            using (var client = new SmtpClient())
            {
                client.Send(message);
            }
        }
    }
}