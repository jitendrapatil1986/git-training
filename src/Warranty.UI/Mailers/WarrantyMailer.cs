using System;
using Mvc.Mailer;
using Warranty.Core;
using Warranty.Core.Features.CreateServiceCall;

namespace Warranty.UI.Mailer
{
    public class WarrantyMailer : MailerBase, IWarrantyMailer
    {
        private readonly IMediator _mediator;

        public WarrantyMailer(IMediator mediator)
        {
            _mediator = mediator;
            MasterName = "_Layout";
        }

        public MvcMailMessage NewServiceCallAssignedToWsr(Guid serviceCallId)
        {
            var serviceCallModel = _mediator.Request(new NewServiceCallAssignedToWsrNotificationQuery
            {
                ServiceCallId = serviceCallId
            });

            ViewBag.HomeOwnerName = serviceCallModel.HomeOwnerName;
            ViewBag.HomePhone = serviceCallModel.HomePhone;
            ViewBag.CommunityName = serviceCallModel.CommunityName;
            ViewBag.AddressLine = serviceCallModel.AddressLine;
            ViewBag.Comments = serviceCallModel.Comments;

            return Populate(x =>
                {
                    x.Subject = string.Format("Warranty Call # {0}", serviceCallModel.ServiceCallNumber);
                    x.ViewName = "NewServiceCallAssignedToWsr";
                    x.To.Add(serviceCallModel.WarrantyRepresentativeEmployeeEmail);
                });
        }
    }
}