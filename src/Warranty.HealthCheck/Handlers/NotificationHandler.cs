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
            _log.Info(message.Body);
        }
    }

    public class Notification : IBusCommand
    {
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}