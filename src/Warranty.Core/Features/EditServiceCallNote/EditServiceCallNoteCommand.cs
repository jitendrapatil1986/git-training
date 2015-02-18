namespace Warranty.Core.Features.EditServiceCallNote
{
    using System;

    public class EditServiceCallNoteCommand : ICommand
    {
        public Guid ServiceCallNoteId { get; set; }
        public Guid ServiceCallId { get; set; }
        public string Note { get; set; }
    }
}