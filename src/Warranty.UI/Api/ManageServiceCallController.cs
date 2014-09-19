namespace Warranty.UI.Api
{
    using System;
    using System.Web.Mvc;
    using Mailer;
    using Warranty.Core;
    using Warranty.Core.Features.AddServiceCallLineItem;
    using Warranty.Core.Features.AddServiceCallNote;
    using Warranty.Core.Features.EditServiceCallLineItem;
    using Warranty.Core.Security;

    public class ManageServiceCallController: ApiController
    {
        private readonly IMediator _mediator;

        public ManageServiceCallController(IMediator mediator, IWarrantyMailer mailer, IUserSession userSession)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public Guid EditLineItem(EditServiceCallLineModel model)
        {
            var result = _mediator.Send(new EditServiceCallLineCommand
            {
                ServiceCallLineItemId = model.ServiceCallLineItemId,
                ProblemCode = model.ProblemCode,
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
                ProblemDescription = model.ProblemDescription
            });

            return resultModel;
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