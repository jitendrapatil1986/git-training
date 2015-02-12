namespace Warranty.Core.Features.DeleteServiceCallNote
{
    using System;

    public class DeleteServiceCallNoteCommand : ICommand
    {
        public Guid ServiceCallNoteId { get; set; }
        public Guid ServiceCallId { get; set; }
        public string Note { get; set; }
    }
}