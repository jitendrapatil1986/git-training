using Mvc.Mailer;
using Warranty.Core.Features.CreateServiceCall;

namespace Warranty.UI.Mailer
{
    public interface IWarrantyMailer
    {
        MvcMailMessage NewServiceCallAssignedToWsr(NewServiceCallAssignedToWsrNotificationModel model);
    }
}