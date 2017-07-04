using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warranty.Core.Entities;

namespace Warranty.Core.Features.SetDefaultWidgetSize
{
   public class SetDefaultWidgetSizeCommand : ICommand<UserSettings>
    {
        public Guid EmployeeId { get; set; }
        public int ServiceCallWidgetSize { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
