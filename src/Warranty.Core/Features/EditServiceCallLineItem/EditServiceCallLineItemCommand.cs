namespace Warranty.Core.Features.EditServiceCallLineItem
{
    using System;

    public class EditServiceCallLineCommand : ICommand<Guid>
    {
        public Guid ServiceCallLineItemId { get; set; }
        public string ProblemCode { get; set; }
        public string ProblemDetailCode { get; set; }
        public string ProblemJdeCode { get; set; }
        public string ProblemDescription { get; set; }
    }
}
