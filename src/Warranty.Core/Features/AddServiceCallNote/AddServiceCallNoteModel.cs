namespace Warranty.Core.Features.AddServiceCallNote
{
    using System;
    using Enumerations;

    public class AddServiceCallNoteModel
    {
        public Guid ServiceCallNoteId { get; set; }
        public Guid ServiceCallId { get; set; }
        public string Note { get; set; }
        public Guid? ServiceCallLineItemId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public ServiceCallLineItemStatus ServiceCallLineItemStatus { get; set; }
    }
}