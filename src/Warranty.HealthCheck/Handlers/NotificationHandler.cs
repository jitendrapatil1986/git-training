using Common.Messages;
using NServiceBus;

namespace Warranty.HealthCheck.Handlers
{
    public class NotificationHandler : IHandleMessages<Notification>
    {
        public void Handle(Notification message)
        {
            
        }
    }

    public class Notification : IBusCommand
    {
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}