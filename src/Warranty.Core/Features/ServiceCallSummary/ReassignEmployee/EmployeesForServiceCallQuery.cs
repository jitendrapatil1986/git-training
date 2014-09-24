using System;
using System.Collections.Generic;

namespace Warranty.Core.Features.ServiceCallSummary.ReassignEmployee
{
    public class EmployeesForServiceCallQuery : IQuery<List<ServiceCallSummaryModel.EmployeeViewModel>>
    {
        public Guid ServiceCallId { get; set; }
    }
}