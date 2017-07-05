using Warranty.Core.Features.ServiceCallsWidget;

namespace Warranty.Core.Features.SetDefaultWidgetSize
{
    public class ServiceCallWidgetSizeViewModel
    {
        public Entities.UserSettings WidgetSize { get; set; }
        public ServiceCallsWidgetModel WidgetModel { get; set; }
    }
}
