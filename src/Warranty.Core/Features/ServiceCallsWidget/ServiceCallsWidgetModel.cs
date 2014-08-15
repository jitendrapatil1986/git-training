namespace Warranty.Core.Features.ServiceCallsWidget
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ServiceCallsWidgetModel
    {
        public ServiceCallsWidgetModel()
        {
            MyServiceCalls = new List<ServiceCall>();
            OverdueServiceCalls = new List<ServiceCall>();
            SpecialProjectServiceCalls = new List<ServiceCall>();
            EscalatedServiceCalls = new List<ServiceCall>();
        }

        public IEnumerable<ServiceCall> MyServiceCalls { get; set; }
        public IEnumerable<ServiceCall> OverdueServiceCalls { get; set; }
        public IEnumerable<ServiceCall> SpecialProjectServiceCalls { get; set; }
        public IEnumerable<ServiceCall> EscalatedServiceCalls { get; set; }

        public IEnumerable<RepresentativeWithCallCount> RepresentativesWithOverdueCalls
        {
            get
            {
                return OverdueServiceCalls.GroupBy(call => call.AssignedToEmployeeNumber
                                                         , call => call.AssignedTo,
                                                           (key, g) =>
                                                           new RepresentativeWithCallCount
                                                               {
                                                                   EmployeeNumber = key,
                                                                   Name = g.First().ToLower(),
                                                                   ServiceCallsCount = g.Count()
                                                               })
                                          .OrderByDescending(x=>x.ServiceCallsCount);
            }
        }

        public IEnumerable<RepresentativeWithCallCount> RepresentativeWithSpecialProjectCalls
        {
            get
            {
                return SpecialProjectServiceCalls.GroupBy(call => call.AssignedToEmployeeNumber
                                                          , call => call.AssignedTo,
                                                          (key, g) =>
                                                          new RepresentativeWithCallCount
                                                              {
                                                                  EmployeeNumber = key,
                                                                  Name = g.First().ToLower(),
                                                                  ServiceCallsCount = g.Count()
                                                              })
                                                 .OrderByDescending(x => x.ServiceCallsCount);
            }
        }

        public IEnumerable<RepresentativeWithCallCount> RepresentativeWithEscalatedCalls
        {
            get
            {
                return EscalatedServiceCalls.GroupBy(call => call.AssignedToEmployeeNumber
                                                          , call => call.AssignedTo,
                                                          (key, g) =>
                                                          new RepresentativeWithCallCount
                                                          {
                                                              EmployeeNumber = key,
                                                              Name = g.First().ToLower(),
                                                              ServiceCallsCount = g.Count()
                                                          })
                                                 .OrderByDescending(x => x.ServiceCallsCount);
            }
        }

        public class RepresentativeWithCallCount
        {
            public string EmployeeNumber { get; set; }
            public string Name { get; set; }
            public int ServiceCallsCount { get; set; }
        }

        public class ServiceCall
        {
            public Guid ServiceCallId { get; set; }
            public string AssignedTo { get; set; }
            public string AssignedToEmployeeNumber { get; set; }
            public string Address { get; set; }
            public string CallNumber { get; set; }
            public DateTime CreatedDate { get; set; }
            public string HomeownerName { get; set; }
            public int NumberOfDaysRemaining { get; set; }
            public int NumberOfLineItems { get; set; }
            public string PhoneNumber { get; set; }
            public DateTime? EscalationDate { get; set; }
            public string EscalationReason { get; set; }

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
