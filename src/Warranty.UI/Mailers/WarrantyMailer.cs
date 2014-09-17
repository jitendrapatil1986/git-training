using Mvc.Mailer;
using Warranty.Core;
using Warranty.Core.Features.CreateServiceCall;

namespace Warranty.UI.Mailer
{
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
    }
}