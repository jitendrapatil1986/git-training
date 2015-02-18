namespace Warranty.Core.Features.EditServiceCallLineItemNote
{
    using System;

    public class EditServiceCallLineItemNoteCommand : ICommand
    {
        public Guid ServiceCallNoteId { get; set; }
        public Guid ServiceCallLineItemId { get; set; }
        public string Note { get; set; }
    }
}