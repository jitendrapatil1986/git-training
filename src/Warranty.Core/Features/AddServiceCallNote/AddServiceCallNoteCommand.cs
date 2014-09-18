namespace Warranty.Core.Features.AddServiceCallNote
{
    using System;

    public class AddServiceCallNoteCommand : ICommand<AddServiceCallNoteModel>
    {
        public Guid ServiceCallId { get; set; }
        public string Note { get; set; }
        public Guid? ServiceCallLineItemId { get; set; }
    }
}