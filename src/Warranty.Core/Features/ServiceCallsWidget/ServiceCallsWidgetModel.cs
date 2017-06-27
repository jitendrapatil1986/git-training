namespace Warranty.Core.Features.ServiceCallsWidget
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Services;
  

    public class ServiceCallsWidgetModel
    {      
        public ServiceCallsWidgetModel()
        {
           

            MyServiceCalls = new List<ServiceCall>();
            OpenServiceCalls = new List<ServiceCall>();
            SpecialProjectServiceCalls = new List<ServiceCall>();
            EscalatedServiceCalls = new List<ServiceCall>();
            ClosedServiceCalls = new List<ServiceCall>();
           
        }

        public IEnumerable<ServiceCall> MyServiceCalls { get; set; }
        public IEnumerable<ServiceCall> OpenServiceCalls { get; set; }
        public IEnumerable<ServiceCall> SpecialProjectServiceCalls { get; set; }
        public IEnumerable<ServiceCall> EscalatedServiceCalls { get; set; }
        public IEnumerable<ServiceCall> ClosedServiceCalls { get; set; } 
        public int WidgetSize { get; set; }


        public IEnumerable<RepresentativeWithCallCount> RepresentativesWithOpenCalls
        {
            get
            {
                return GetRepresentativeWithCallCount(OpenServiceCalls);
            }
        }

        public IEnumerable<RepresentativeWithCallCount> RepresentativeWithSpecialProjectCalls
        {
            get
            {
                return GetRepresentativeWithCallCount(SpecialProjectServiceCalls);
            }
        }

        public IEnumerable<RepresentativeWithCallCount> RepresentativeWithEscalatedCalls
        {
            get
            {
                return GetRepresentativeWithCallCount(EscalatedServiceCalls);
            }
        }

        private IEnumerable<RepresentativeWithCallCount> GetRepresentativeWithCallCount(IEnumerable<ServiceCall> calls)
        {
            return calls.GroupBy(call => call.AssignedToEmployeeNumber, call => call.AssignedTo,
                                 (key, g) =>
                                 new RepresentativeWithCallCount
                                     {
                                         EmployeeNumber = key,
                                         Name = g.First().ToLower(),
                                         ServiceCallsCount = g.Count()
                                     })
                        .OrderByDescending(x => x.ServiceCallsCount);
        }

      
        public class UserSettings
        {
            public int WidgetSize_Id { get; set; }
            public int ServiceCallWidgetSize { get; set; }
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
            public Guid JobId { get; set; }
            public string AssignedTo { get; set; }
            public string AssignedToEmployeeNumber { get; set; }
            public string Address { get; set; }
            public string CallNumber { get; set; }
            public string JobNumber { get; set; }
            public DateTime CreatedDate { get; set; }
            public string HomeownerName { get; set; }
            public int HomeownerNumber { get; set; }
            public int NumberOfDaysRemaining { get { return ServiceCallCalculator.CalculateNumberOfDaysRemaining(CreatedDate); } }
            public int NumberOfLineItems { get; set; }
            public string PhoneNumber { get; set; }
            public DateTime? EscalationDate { get; set; }
            public string EscalationReason { get; set; }
            public bool IsSpecialProject { get; set; }
            public bool IsEscalated { get; set; }
            public int DaysOpenedFor { get; set; }
            public DateTime? CompletionDate { get; set; }
            


            public int PercentComplete
            {
                get { return ServiceCallCalculator.CalculatePercentComplete(NumberOfDaysRemaining); }
            }

            public int YearsWithinWarranty { get; set; }
            public DateTime WarrantyStartDate { get; set; }
        }
    }
}
