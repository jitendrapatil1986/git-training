namespace Warranty.Events
{
    using System;
    using NServiceBus;

    public class ServiceCallLineItemCreated : IEvent
    {
        public Guid ServiceCallId { get; set; }
        public Guid ServiceCallLineItemId { get; set; }
        public int LineNumber { get; set; }
        public string ProblemCode { get; set; }
        public string ProblemDescription { get; set; }
        public string CauseDescription { get; set; }
        public string ClassificationNote { get; set; }
        public string LineItemRoot { get; set; }
        public string ServiceCallLineItemStatus { get; set; }
    }
}