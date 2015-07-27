using System.Configuration;

namespace Warranty.UI.Mailers
{
    using Mvc.Mailer;
    using Warranty.Core.Extensions;
    using Warranty.Core.Features.CreateServiceCall;
    using Warranty.Core.Features.ServiceCallToggleActions;
    using Common.Security.Session;

    public class WarrantyMailer : MailerBase, IWarrantyMailer
    {
        private IUserSession _userSession;

        public WarrantyMailer(IUserSession userSession)
        {
            MasterName = "_Layout";
            _userSession = userSession;
        }

        public MvcMailMessage NewServiceCallAssignedToWsr(NewServiceCallAssignedToWsrNotificationModel model)
        {
            ViewBag.HomeOwnerName = model.HomeOwnerName;
            ViewBag.HomePhone = model.HomePhone;
            ViewBag.CommunityName = model.CommunityName;
            ViewBag.AddressLine = model.AddressLine;
            ViewBag.Comments = model.Comments;
            ViewBag.ServiceCallNumber = model.ServiceCallNumber;
            ViewBag.Url = model.Url;

            if (!ConfigurationManager.AppSettings["sendEmailsForTest"].IsNullOrEmpty())
            {
                model.WarrantyRepresentativeEmployeeEmail = _userSession.GetActualUser().Email;
            }

            return Populate(x =>
            {
                x.Subject = string.Format("Warranty Call # {0}", model.ServiceCallNumber);
                x.ViewName = "NewServiceCallAssignedToWsr";
                x.To.Add(model.WarrantyRepresentativeEmployeeEmail);
            });
        }

        public MvcMailMessage ServiceCallCompleted(ServiceCallCompleteWsrNotificationModel model)
        {
            ViewBag.HomeOwnerName = model.HomeOwnerName;
            ViewBag.HomePhone = model.HomePhone;
            ViewBag.CommunityName = model.CommunityName;
            ViewBag.AddressLine = model.AddressLine;
            ViewBag.Comments = model.Comments;
            ViewBag.ServiceCallNumber = model.ServiceCallNumber;
            ViewBag.Url = model.Url;

            if (!ConfigurationManager.AppSettings["sendEmailsForTest"].IsNullOrEmpty())
            {
                model.WarrantyRepresentativeEmployeeEmail = _userSession.GetActualUser().Email;
            }

            return Populate(x =>
            {
                x.Subject = string.Format("Warranty Call # {0} has been completed.", model.ServiceCallNumber);
                x.ViewName = "ServiceCallCompleted";
                x.To.Add(model.WarrantyRepresentativeEmployeeEmail);
            });
        }
        
        public MvcMailMessage ServiceCallEscalated(ServiceCallToggleEscalateCommandResult model)
        {
            ViewBag.HomeOwnerName = model.HomeOwnerName;
            ViewBag.HomePhone = model.HomePhone;
            ViewBag.CommunityName = model.CommunityName;
            ViewBag.AddressLine = model.AddressLine;
            ViewBag.Comments = model.Comments;
            ViewBag.ServiceCallNumber = model.CallNumber;
            ViewBag.Url = model.Url;

            if (!ConfigurationManager.AppSettings["sendEmailsForTest"].IsNullOrEmpty())
                model.Emails = new[] { _userSession.GetActualUser().Email };

            return Populate(x =>
            {
                x.Subject = string.Format("Warranty Call # {0} has been escalated.", model.CallNumber);
                x.ViewName = "ServiceCallEscalated";
                x.To.Add(model.Emails.CommaSeparate());
            });
        }
    }
}