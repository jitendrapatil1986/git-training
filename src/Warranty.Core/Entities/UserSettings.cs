using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warranty.Core.Entities
{
    public class UserSettings : IAuditableEntity
    {
        public Guid UserSettingsId { get; set; }
        public Guid EmployeeId { get; set; }
        public Int32 ServiceCallWidgetSize { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual string UpdatedBy { get; set; }
    }
}
