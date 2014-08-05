namespace Warranty.Core.Entities
{
    using System;

    public interface IAuditableEntity
    {
        string CreatedBy { get; set; }
        DateTime? CreatedDate { get; set; }
        string UpdatedBy { get; set; }
        DateTime? UpdatedDate { get; set; }
    }
}