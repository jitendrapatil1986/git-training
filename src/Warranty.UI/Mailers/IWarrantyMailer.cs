namespace Warranty.UI.Mailers
{
    using Mvc.Mailer;
    using Warranty.Core.Features.CreateServiceCall;
    using Warranty.Core.Features.ServiceCallToggleActions;

    public interface IWarrantyMailer
    {
        MvcMailMessage NewServiceCallAssignedToWsr(NewServiceCallAssignedToWsrNotificationModel model);
        MvcMailMessage ServiceCallEscalated(ServiceCallToggleEscalateCommandResult model);
    }
}