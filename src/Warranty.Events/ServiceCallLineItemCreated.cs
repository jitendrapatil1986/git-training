namespace Warranty.Events
{
    using NServiceBus;

    public class ServiceCallLineItemCreated : IEvent
    {
        public int ServiceCallNumber { get; set; }
        public int LineNumber { get; set; }
        public string ProblemCode { get; set; }
        public string ProblemDescription { get; set; }
        public string CauseDescription { get; set; }
        public string ClassificationNote { get; set; }
        public string LineItemRoot { get; set; }
        public string ServiceCallLineItemStatus { get; set; }
    }
}