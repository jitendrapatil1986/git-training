using Common.Security.Session;
using NServiceBus.Logging;

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

    public class ManageServiceCallController: ApiController
    {
        private readonly IMediator _mediator;
        private readonly IWarrantyMailer _mailer;
        private readonly ILog _log = LogManager.GetLogger(typeof(ManageServiceCallController));

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
            if (RootProblem.FromDisplayNameOrDefault(model.RootProblem) == null)
                throw new ArgumentException("Root problem must be selected.", "model.RootProblem");

            RootCause rootCause;

            if (!RootCause.TryParse(model.RootCause, out rootCause))
                throw new ArgumentException("Root cause must be selected.", "model.RootCause");

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
            var result = _mediator.Send(model);

            try
            {
                if (!string.IsNullOrEmpty(model.ProjectCoordinatorEmailToNotify))
                {
                    _mailer.NewHomeownerPaymentRequested(model).Send();
                }
            }
            catch (Exception e)
            {
                _log.ErrorFormat("Failed to send email to PC for warranty payment to homeowner created for service call line item: {0} at email: {1}", 
                    model.ServiceCallLineItemId, model.ProjectCoordinatorEmailToNotify);
                _log.ErrorFormat("Exception message: {0}.  Stacktrace: {1}",
                    e.Message, e.StackTrace);
                throw new ApplicationException("EMAIL_SEND_FAILURE");
            }

            return result;
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

        [HttpPost]
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