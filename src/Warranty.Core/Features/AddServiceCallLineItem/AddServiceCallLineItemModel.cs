namespace Warranty.Core.Features.AddServiceCallLineItem
{
    using System;

    public class AddServiceCallLineItemModel
    {
        public Guid ServiceCallLineItemId { get; set; }
        public Guid ServiceCallId { get; set; }
        public string ProblemCode { get; set; }
        public string ProblemDescription { get; set; }
        public int LineNumber { get; set; }
    }
}
