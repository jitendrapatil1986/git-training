namespace Warranty.UI.Api
{
    using System;
    using System.Web.Mvc;
    using Core.Helpers;
    using Mailers;
    using Warranty.Core;
    using Warranty.Core.Enumerations;
    using Warranty.Core.Features.AddServiceCallLineItem;
    using Warranty.Core.Features.AddServiceCallNote;
    using Warranty.Core.Features.AddServiceCallPayment;
    using Warranty.Core.Features.AddServiceCallPurchaseOrder;
    using Warranty.Core.Features.CompleteServiceCallLineItem;
    using Warranty.Core.Features.DeleteServiceCall;
    using Warranty.Core.Features.DeleteServiceCallLineNote;
    using Warranty.Core.Features.DeleteServiceCallNote;
    using Warranty.Core.Features.EditServiceCallLineItem;
    using Warranty.Core.Features.EditServiceCallLineItemNote;
    using Warranty.Core.Features.EditServiceCallNote;
    using Warranty.Core.Features.ServiceCallToggleActions;
    using Warranty.Core.Features.SharedQueries;
    using Warranty.Core.Features.UpdateServiceCallLineItem;
    using Warranty.Core.Features.EditServiceCallStatus;
    using Warranty.Core.Features.NoActionServiceCallLineItem;
    using Warranty.Core.Security;

    public class ManageServiceCallController: ApiController
    {
        private readonly IMediator _mediator;
        private readonly IWarrantyMailer _mailer;

        public ManageServiceCallController(IMediator mediator, IWarrantyMailer mailer, IUserSession userSession)
        {
            _mediator = mediator;
            _mailer = mailer;
        }

        [HttpPost]
        public VerifyHomeownerSignatureAndCloseCallModel VerifyHomeownerSignatureAndCloseCall(VerifyHomeownerSignatureAndCloseCallCommand model)
        {
            var resultModel = _mediator.Send(model);

            var notificationModel = _mediator.Request(new ServiceCallCompleteWsrNotificationQuery { ServiceCallId = model.ServiceCallId });
            if (notificationModel.WarrantyRepresentativeEmployeeEmail != null)
            {
                notificationModel.Url = UrlBuilderHelper.GetUrl("ServiceCall", "CallSummary", new { id = model.ServiceCallId });
                _mailer.ServiceCallCompleted(notificationModel).SendAsync();
            }

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
        public ServiceCallLineItemStatus NoActionLineItem(NoActionServiceCallLineItemModel model)
        {
            var result = _mediator.Send(new NoActionServiceCallLineItemCommand
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
        public AddStandAloneBackchargeCommandDto AddStandAloneBackcharge(AddStandAloneBackchargeCommand model)
        {
            return _mediator.Send(model); ;
        }

        [System.Web.Http.HttpDelete]
        public string DeleteStandAloneBackcharge(DeleteBackchargeCommand model)
        {
            _mediator.Send(model);
            return "success";
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
        public PostResponseModel AdminDeleteServiceCall(AdminDeleteServiceCallCommand command)
        {
            _mediator.Send(command);
            return new PostResponseModel { Success = true };
        }

        [HttpDelete]
        public PostResponseModel DeleteServiceCall(DeleteServiceCallCommand command)
        {
            _mediator.Send(command);
            return new PostResponseModel { Success = true };
        }

        [HttpDelete]
        public PostResponseModel DeleteServiceCallNote(DeleteServiceCallNoteCommand model)
        {
            _mediator.Send(model);
            return new PostResponseModel {Success = true};
        }

        [HttpDelete]
        public PostResponseModel DeleteServiceCallLineItemNote(DeleteServiceCallLineNoteCommand model)
        {
            _mediator.Send(model);
            return new PostResponseModel { Success = true };
        }

        [HttpPost]
        public PostResponseModel EditServiceCallNote(EditServiceCallNoteCommand model)
        {
            _mediator.Send(model);
            return new PostResponseModel { Success = true };
        }

        [HttpPost]
        public PostResponseModel EditServiceCallLineItemNote(EditServiceCallLineItemNoteCommand model)
        {
            _mediator.Send(model);
            return new PostResponseModel { Success = true };
        }
    }
}