using System;
using System.Collections.Generic;
using System.Web.Http;
using Warranty.Core;
using Warranty.Core.Features.AssignWSRs;
using Warranty.Core.Features.CreateServiceCallCustomerSearch;
using Warranty.Core.Features.QuickSearch;

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
        public void AddAssignment(string communityId, string employeeId)
        {
            _mediator.Send(new AssignWSRCommand{CommunityId = Guid.Parse(communityId), EmployeeId = Guid.Parse(employeeId)});
        }
    }
}