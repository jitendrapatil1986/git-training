namespace Warranty.UI.Api
{
    using System.Web.Mvc;
    using Warranty.Core;
    using Warranty.Core.Features.AddJobNote;
    using Warranty.Core.Features.JobSummary.ChangeHomeowner;
    using Warranty.Core.Features.SharedQueries;

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

        [HttpPost]
        public PostResponseModel ChangeHomeowner(ChangeHomeownerModel model)
        {
            _mediator.Send(new ChangeHomeownerCommand
            {
                Model = model,
            });

            return new PostResponseModel { Success = true };
        }
    }
}