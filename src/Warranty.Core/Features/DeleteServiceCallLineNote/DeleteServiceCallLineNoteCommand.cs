namespace Warranty.Core.Features.DeleteServiceCallLineNote
{
    using System;

    public class DeleteServiceCallLineNoteCommand : ICommand
    {
        public Guid ServiceCallNoteId { get; set; }
        public Guid ServiceCallLineItemId { get; set; }
        public string Note { get; set; }
    }
}