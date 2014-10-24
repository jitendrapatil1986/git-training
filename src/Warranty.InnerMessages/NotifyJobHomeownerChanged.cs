namespace Warranty.InnerMessages
{
    using System;
    using NServiceBus;

    public class NotifyJobHomeownerChanged : ICommand
    {
        public Guid JobId { get; set; }
    }
}