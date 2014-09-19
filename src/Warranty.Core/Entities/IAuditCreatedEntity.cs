using System;

namespace Warranty.Core.Entities
{
    public interface IAuditCreatedEntity
    {
        string CreatedBy { get; set; }
        DateTime CreatedDate { get; set; }
    }
}