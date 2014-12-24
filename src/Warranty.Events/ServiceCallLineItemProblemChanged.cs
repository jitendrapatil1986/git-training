namespace Warranty.Events
{
    using System;
    using NServiceBus;

    public class ServiceCallLineItemProblemChanged : IEvent
    {
        public Guid ServiceCallId { get; set; }
        public Guid ServiceCallLineItemId { get; set; }
        public string ProblemCode { get; set; }
        public string ProbemDescription { get; set; }
        public string ProblemJdeCode { get; set; }
        public string RootCause { get; set; }
        public string RootProblem { get; set; }
    }
}