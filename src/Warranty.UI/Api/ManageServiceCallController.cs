namespace Warranty.UI.Api
{
    using System;
    using System.Web.Mvc;
    using Mailers;
    using Warranty.Core;
    using Warranty.Core.Enumerations;
    using Warranty.Core.Features.AddServiceCallLineItem;
    using Warranty.Core.Features.AddServiceCallNote;
    using Warranty.Core.Features.CompleteServiceCallLineItem;
    using Warranty.Core.Features.EditServiceCallLineItem;
	using Warranty.Core.Features.UpdateServiceCallLineItem;
    using Warranty.Core.Features.EditServiceCallStatus;
    using Warranty.Core.Security;

    public class ManageServiceCallController: ApiController
    {
        private readonly IMediator _mediator;

        public ManageServiceCallController(IMediator mediator, IWarrantyMailer mailer, IUserSession userSession)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public VerifyHomeownerSignatureServiceCallStatusModel VerifyHomeownerSignatureServiceCall(VerifyHomeownerSignatureServiceCallStatusModel model)
        {
            var resultModel = _mediator.Send(new VerifyHomeownerSignatureServiceCallStatusCommand
                {
                    ServiceCallId = model.ServiceCallId,
                    ServiceCallStatus = model.ServiceCallStatus,
                    HomeownerVerificationSignature = model.HomeownerVerificationSignature,
                    HomeownerVerificationSignatureDate = model.HomeownerVerificationSignatureDate,
                });

            return resultModel;
        }

        [HttpPost]
        public Guid EditLineItem(EditServiceCallLineModel model)
        {
            var result = _mediator.Send(new EditServiceCallLineCommand
            {
                ServiceCallLineItemId = model.ServiceCallLineItemId,
                ProblemCode = model.ProblemCode,
                RootCause = model.RootCause,
                ProblemDescription = model.ProblemDescription
            });

            return result;
        }

        [HttpPost]
        public AddServiceCallLineItemModel AddLineItem(AddServiceCallLineItemModel model)
        {
            var resultModel = _mediator.Send(new AddServiceCallLineItemCommand
            {
                ServiceCallId = model.ServiceCallId,
                ProblemCode = model.ProblemCode,
                RootCause = model.RootCause,
                ProblemDescription = model.ProblemDescription
            });

            return resultModel;
        }

        [HttpPost]
        public ServiceCallLineItemStatus CompleteLineItem(CompleteServiceCallLineItemModel model)
        {
            var result = _mediator.Send(new CompleteServiceCallLineItemCommand
                {
                    ServiceCallLineItemId = model.ServiceCallLineItemId
                });

            return result;
        }

        [HttpPost]
        public ServiceCallLineItemStatus ReopenLineItem(UpdateServiceCallLineItemModel model)
        {
            var result = _mediator.Send(new UpdateServiceCallLineItemCommand
                {
                    ServiceCallLineItemId = model.ServiceCallLineItemId,
                });

            return result;
        }

        [HttpPost]
        public AddServiceCallNoteModel AddNote(AddServiceCallNoteModel model)
        {
            var resultModel = _mediator.Send(new AddServiceCallNoteCommand
            {
                ServiceCallId = model.ServiceCallId,
                ServiceCallLineItemId = model.ServiceCallLineItemId,
                Note = model.Note
            });

            return resultModel;
        }
    }
}