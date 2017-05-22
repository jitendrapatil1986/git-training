using System.Collections.Generic;
using System.Configuration;
using Common.Extensions;
using Warranty.Core;
using Warranty.Core.Features.AddServiceCallPayment;
using Warranty.Core.Features.Homeowner;
using Warranty.Core.Features.Job;
using Warranty.Core.Features.ServiceCallSummary.ServiceCallLineItem;
using Warranty.UI.Core.Helpers;

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
        private IMediator _mediator;

        public WarrantyMailer(IUserSession userSession, IMediator mediator)
        {
            MasterName = "_Layout";
            _userSession = userSession;
            _mediator = mediator;
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
            if (model.LineItems != null && model.LineItems.Count > 0)
            {
                ViewBag.LineItems = model.LineItems;
            }
            else
            {
                ViewBag.LineItems = null;
            }           

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

        public MvcMailMessage NewHomeownerPaymentRequested(AddPaymentCommand model)
        {
            var lineItem = _mediator.Request(new ServiceCallLineItemQuery {ServiceCallLineItemId = model.ServiceCallLineItemId});
            var job = _mediator.Request(new GetJobQuery(lineItem.JobNumber));
            var homeowner = _mediator.Request(new GetHomeOwnerQuery(job.JobNumber));
            
            ViewBag.ServiceCallNumber = lineItem.ServiceCallNumber;
            ViewBag.Url = UrlBuilderHelper.GetUrl("ServiceCall", "LineItemDetail", new { id = model.ServiceCallLineItemId });
            ViewBag.LineItem = lineItem.ProblemDescription;
            ViewBag.HomeOwnerName = homeowner.HomeOwnerName;
            ViewBag.JobNumber = lineItem.JobNumber;
            ViewBag.AddressLine = job.AddressLine;
            ViewBag.HomeownerEmail = homeowner.EmailAddress;
            ViewBag.HomePhone = homeowner.HomePhone.ToPhoneNumber();
            ViewBag.Amount = model.Amount.ToString("C");
            ViewBag.Comments = model.Comments;
            ViewBag.CheckDestination = model.SendCheckToProjectCoordinator ? "PC" : "Homeowner";
            
            var emailRecipient = model.ProjectCoordinatorEmailToNotify;
            var requester = _userSession.GetActualUser().LastName + ", " + _userSession.GetActualUser().FirstName;

            if (!ConfigurationManager.AppSettings["sendEmailsForTest"].IsNullOrEmpty())
                emailRecipient = _userSession.GetActualUser().Email;

            MasterName = "_simpleLayout";

            return Populate(x =>
            {
                x.Subject = string.Format("Warranty payment for homeowner requested by {0}", requester);
                x.ViewName = "NewHomeownerPaymentRequested";
                x.To.Add(emailRecipient);
            });
        }
    }
}