namespace Warranty.Core.Entities
{
    using System;

    public class JobAttachment : IAuditableEntity
    {
        public Guid JobAttachmentId { get; set; }
        public Guid JobId { get; set; }
        public string FilePath { get; set; }
        public string DisplayName { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}