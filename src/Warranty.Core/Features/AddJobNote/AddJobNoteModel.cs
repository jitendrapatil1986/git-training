namespace Warranty.Core.Features.AddJobNote
{
    using System;

    public class AddJobNoteModel
    {
        public Guid JobNoteId { get; set; }
        public Guid JobId { get; set; }
        public string Note { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}