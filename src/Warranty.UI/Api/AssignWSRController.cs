using System.Web.Http;
using Warranty.Core;
using Warranty.Core.Features.AssignWSRs;
using Warranty.Core.Features.SharedQueries;

namespace Warranty.UI.Api
{
    public class AssignWSRController : ApiController
    {
        private readonly IMediator _mediator;

        public AssignWSRController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public PostResponseModel RemoveAssignment(RemoveAssignmentModel assignment)
        {   
            _mediator.Send(new RemoveAssignmentCommand
                {
                    AssignmentId = assignment.AssignmentId
                });

            return new PostResponseModel { Success = true };
        }

        [HttpPost]
        public PostResponseModel UpdateAssignment(AssignWSRsModel assignment)
        {
            _mediator.Send(new AssignWSRCommand
            {
                CommunityId = assignment.SelectedCommunityId,
                EmployeeId = assignment.SelectedEmployeeId,
            });

            return new PostResponseModel { Success = true };
        }
    }
}