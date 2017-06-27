using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warranty.Core.Features.ServiceCallsWidget;

namespace Warranty.Core.Features.SetDefaultWidgetSize
{
    public class ServiceCallWidgetSizeViewModel
    {
        public UserSettings widgetSize { get; set; }
        public ServiceCallsWidgetModel widgetModel { get; set; }
    }
}
