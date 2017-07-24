using NServiceBus;
using System;

namespace Warranty.InnerMessages
{
    public class NotifyLineItemDeleted : ICommand
    {
        public Guid ServiceCallLineItemId { get; set; }
    }
}
