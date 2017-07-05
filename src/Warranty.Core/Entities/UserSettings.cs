using System;

namespace Warranty.Core.Entities
{
    public class UserSettings : IAuditableEntity
    {
        public Guid UserSettingsId { get; set; }
        public Guid EmployeeId { get; set; }
        public int ServiceCallWidgetSize { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
    }
}
