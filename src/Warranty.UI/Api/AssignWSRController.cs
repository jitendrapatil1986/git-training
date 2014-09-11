using System.Web.Http;
using Warranty.Core;
using Warranty.Core.Features.AssignWSRs;

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
        public bool AddAssignment(AddAssignmentModel newAddAssignment)
        {
            _mediator.Send(new AssignWSRCommand
            {
                CommunityId = newAddAssignment.CommunityId,
                EmployeeId = newAddAssignment.EmployeeId
            });
            return true;
        }

        [HttpPost]
        public bool RemoveAssignment(RemoveAssignmentModel assignment)
        {   
            _mediator.Send(new RemoveAssignmentCommand
                {
                    AssignmentId = assignment.AssignmentId
                });
            return true;
        }
    }
}