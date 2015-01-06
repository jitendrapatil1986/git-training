namespace Warranty.UI.Api
{
    using System;
    using System.Web.Mvc;
    using Mailers;
    using Warranty.Core;
    using Warranty.Core.Enumerations;
    using Warranty.Core.Features.AddServiceCallLineItem;
    using Warranty.Core.Features.AddServiceCallNote;
    using Warranty.Core.Features.AddServiceCallPayment;
    using Warranty.Core.Features.AddServiceCallPurchaseOrder;
    using Warranty.Core.Features.CompleteServiceCallLineItem;
    using Warranty.Core.Features.DeleteServiceCall;
    using Warranty.Core.Features.EditServiceCallLineItem;
    using Warranty.Core.Features.SharedQueries;
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
        public VerifyHomeownerSignatureServiceCallStatusModel VerifyHomeownerSignatureServiceCall(VerifyHomeownerSignatureServiceCallStatusCommand model)
        {
            var resultModel = _mediator.Send(model);

            return resultModel;
        }

        [HttpPost]
        public Guid EditLineItem(EditServiceCallLineCommand model)
        {
            var result = _mediator.Send(model);

            return result;
        }

        [HttpPost]
        public AddServiceCallLineItemModel AddLineItem(AddServiceCallLineItemModel model)
        {
            var resultModel = _mediator.Send(new AddServiceCallLineItemCommand
            {
                ServiceCallId = model.ServiceCallId,
                ProblemCode = model.ProblemCode,
                ProblemJdeCode = model.ProblemJdeCode,
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
        public UpdateServiceCallLineItemModel ReopenLineItem(UpdateServiceCallLineItemModel model)
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

        [HttpPost]
        public AddPaymentCommandDto AddPayment(AddPaymentCommand model)
        {
            return _mediator.Send(model); ;
        }

        [System.Web.Http.HttpDelete]
        public string DeletePayment(DeletePaymentCommand model)
        {
            _mediator.Send(model);
            return "success";
        }

        [HttpPost]
        public string ApprovePayment(ApprovePaymentCommand model)
        {
            var response = _mediator.Send(model);
            return response;
        }

        [HttpPost]
        public HoldPaymentCommandHandler.HoldPaymentCommandHandlerResponse AddPaymentOnHold(HoldPaymentCommand model)
        {
            var response = _mediator.Send(model);
            return response;
        }

        [HttpPost]
        public PostResponseModel AddPurchaseOrder(AddServiceCallPurchaseOrderCommand model)
        {
            _mediator.Send(model);
            return new PostResponseModel {Success = true};
        }

        [HttpPost]
        public string ApproveBackcharge(ApproveBackchargeCommand model)
        {
            var response = _mediator.Send(model);
            return response;
        }

        [HttpPost]
        public HoldBackchargeCommandHandler.HoldBackchargeCommandHandlerResponse HoldBackcharge(HoldBackchargeCommand model)
        {
            var response = _mediator.Send(model);
            return response;
        }

        [HttpPost]
        public DenyBackchargeCommandHandler.DenyBackchargeCommandHandlerResponse DenyBackcharge(DenyBackchargeCommand model)
        {
            var response = _mediator.Send(model);
            return response;
        }

        [HttpDelete]
        public PostResponseModel DeleteServiceCall(DeleteServiceCallCommand command)
        {
            _mediator.Send(command);
            return new PostResponseModel { Success = true };
        }
    }
}