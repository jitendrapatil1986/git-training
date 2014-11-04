namespace Warranty.Core.Features.AddServiceCallLineItem
{
    using System;

    public class AddServiceCallLineItemCommand : ICommand<AddServiceCallLineItemModel>
    {
        public Guid ServiceCallId { get; set; }
        public string ProblemCode { get; set; }
        public string ProblemDetailCode { get; set; }
        public string ProblemJdeCode { get; set; }
        public string ProblemDescription { get; set; }
    }
}