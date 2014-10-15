namespace Warranty.Events
{
    using NServiceBus;

    public class ServiceCallLineItemProblemChanged : IEvent
    {
        public int ServiceCallNumber { get; set; }
        public int LineNumber { get; set; }
        public string ProblemCode { get; set; }
        public string ProbemDescription { get; set; }
    }
}