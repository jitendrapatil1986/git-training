namespace Warranty.UI.Api
{
    using System.Web.Mvc;
    using Warranty.Core;
    using Warranty.Core.Features.AddJobNote;

    public class ManageJobController: ApiController
    {
        private readonly Mediator _mediator;

        public ManageJobController(Mediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public AddJobNoteModel AddNote(AddJobNoteModel model)
        {
            var resultModel = _mediator.Send(new AddJobNoteCommand
                {
                    JobId = model.JobId,
                    Note = model.Note,
                });

            return resultModel;
        }
    }
}