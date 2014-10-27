namespace Warranty.Core.Features.EditServiceCallLineItem
{
    using System;

    public class EditServiceCallLineCommand : ICommand<Guid>
    {
        public Guid ServiceCallLineItemId { get; set; }
        public string ProblemCode { get; set; }
        public string RootCause { get; set; }
        public string ProblemDescription { get; set; }
    }
}
