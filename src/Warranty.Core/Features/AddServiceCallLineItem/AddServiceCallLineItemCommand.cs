namespace Warranty.Core.Features.AddServiceCallLineItem
{
    using System;

    public class AddServiceCallLineItemCommand : ICommand<Guid>
    {
        public Guid ServiceCallId { get; set; }
        public string ProblemCode { get; set; }
        public string ProblemDescription { get; set; }
    }
}