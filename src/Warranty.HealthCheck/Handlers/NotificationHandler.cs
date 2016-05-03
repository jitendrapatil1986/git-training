using System.Configuration;
using System.Net.Mail;
using Common.Messages;
using log4net;
using NServiceBus;

namespace Warranty.HealthCheck.Handlers
{
    public class NotificationHandler : IHandleMessages<Notification>
    {
        private readonly ILog _log;

        public NotificationHandler(ILog log)
        {
            _log = log;
        }

        public void Handle(Notification message)
        {
            var addresses = To;
            if (addresses == null)
            {
                _log.Error("The service isn't configured with any addresses in the \"notifications.to\" setting - you need to add one in order for notifications to be sent");
                return;
            }

            var email = new MailMessage
            {
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = true
            };

            foreach (var address in addresses)
                email.To.Add(address);

            using (var client = new SmtpClient())
            {
                client.Send(email);
            }

            _log.Info("Notification sent");
        }

        public string[] To
        {
            get
            {
                var addresses = ConfigurationManager.AppSettings["notifications.to"];
                if (string.IsNullOrWhiteSpace(addresses))
                    return null;

                if (!addresses.Contains(";"))
                    return new [] { addresses };

                return addresses.Split(';');
            }
        }
    }

    public class Notification : IBusCommand
    {
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}