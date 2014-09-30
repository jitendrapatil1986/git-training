namespace Warranty.UI.Mailers
{
    using Mvc.Mailer;
    using Warranty.Core.Extensions;
    using Warranty.Core.Features.CreateServiceCall;
    using Warranty.Core.Features.ServiceCallToggleActions;

    public class WarrantyMailer : MailerBase, IWarrantyMailer
    {
        public WarrantyMailer()
        {
            MasterName = "_Layout";
        }

        public MvcMailMessage NewServiceCallAssignedToWsr(NewServiceCallAssignedToWsrNotificationModel model)
        {
            ViewBag.HomeOwnerName = model.HomeOwnerName;
            ViewBag.HomePhone = model.HomePhone;
            ViewBag.CommunityName = model.CommunityName;
            ViewBag.AddressLine = model.AddressLine;
            ViewBag.Comments = model.Comments;

            return Populate(x =>
            {
                x.Subject = string.Format("Warranty Call # {0}", model.ServiceCallNumber);
                x.ViewName = "NewServiceCallAssignedToWsr";
                x.To.Add(model.WarrantyRepresentativeEmployeeEmail);
            });
        }

        public MvcMailMessage ServiceCallEscalated(ServiceCallToggleEscalateCommandResult model)
        {
            ViewBag.CallNumber = model.CallNumber;
            ViewBag.Url = model.Url;
            return Populate(x =>
            {
                x.Subject = string.Format("Warranty Call # {0} has been escalated.", model.CallNumber);
                x.ViewName = "ServiceCallEscalated";
                x.To.Add(model.Emails.CommaSeparate());
            });
        }
    }
}