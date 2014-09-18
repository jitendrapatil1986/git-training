namespace Warranty.Core.Features.AddServiceCallNote
{
    using System;

    public class AddServiceCallNoteModel
    {
        public Guid ServiceCallNoteId { get; set; }
        public Guid ServiceCallId { get; set; }
        public string Note { get; set; }
        public Guid? ServiceCallLineItemId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}