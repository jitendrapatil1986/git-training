using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warranty.Core.Features.SetDefaultWidgetSize
{
    public class UserSettings
    {
       
        public Guid EmployeeId { get; set; }
        public Int32 ServiceCallWidgetSize { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
