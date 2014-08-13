using System;

namespace Warranty.Core.Features.RepServiceCalls
{
    public class ServiceRepServiceCallsQuery : IQuery<ServiceRepServiceCallsModel>
    {
        public Guid EmployeeId { get; set; }
    }
}
