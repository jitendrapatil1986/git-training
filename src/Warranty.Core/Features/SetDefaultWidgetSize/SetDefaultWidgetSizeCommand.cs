using System;
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
