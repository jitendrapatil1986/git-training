namespace Warranty.UI.Mailers
{
    using Mvc.Mailer;
    using Warranty.Core.Features.CreateServiceCall;
    using Warranty.Core.Features.ServiceCallToggleActions;
    using Warranty.Core.Features.AddServiceCallPayment;

    public interface IWarrantyMailer
    {
        MvcMailMessage NewServiceCallAssignedToWsr(NewServiceCallAssignedToWsrNotificationModel model);
        MvcMailMessage ServiceCallEscalated(ServiceCallToggleEscalateCommandResult model);
        MvcMailMessage ServiceCallCompleted(ServiceCallCompleteWsrNotificationModel model);
        MvcMailMessage NewHomeownerPaymentRequested(AddPaymentCommand projectCoordinatorEmail);
    }
}