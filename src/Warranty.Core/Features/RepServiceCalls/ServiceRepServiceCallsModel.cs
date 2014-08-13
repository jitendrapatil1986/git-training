using System;
using System.Collections.Generic;
using System.Linq;

namespace Warranty.Core.Features.RepServiceCalls
{
    public class ServiceRepServiceCallsModel
    {
        public ServiceRepServiceCallsModel()
        {
            OpenServiceCalls = new List<ServiceCall>();
            ClosedServiceCalls = new List<ServiceCall>();
        }

        public string EmployeeName { get; set; }
        public IEnumerable<ServiceCall> OpenServiceCalls { get; set; }
        public IEnumerable<ServiceCall> ClosedServiceCalls { get; set; }

        public class ServiceCall
        {
            public Guid ServiceCallId { get; set; }
            public string AssignedTo { get; set; }
            public string AssignedToEmployeeNumber { get; set; }
            public string Address { get; set; }
            public string CallNumber { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime? CompletionDate { get; set; }
            public string HomeownerName { get; set; }
            public int NumberOfDaysRemaining { get; set; }
            public int NumberOfLineItems { get; set; }
            public string PhoneNumber { get; set; }

            public int PercentComplete
            {
                get
                {
                    if (NumberOfDaysRemaining == 0)
                        return 100;

                    var complete = (7.0 - NumberOfDaysRemaining)/7.0 * 100;
                    return Convert.ToInt16(complete);
                }
            }
        }
    }
}
