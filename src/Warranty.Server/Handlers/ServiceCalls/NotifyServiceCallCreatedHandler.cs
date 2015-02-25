namespace Warranty.Server.Handlers.ServiceCalls
{
    using Core.Entities;
    using Events;
    using InnerMessages;
    using NPoco;
    using NServiceBus;
    using System.Linq;

    public class NotifyServiceCallCreatedHandler : IHandleMessages<NotifyServiceCallCreated>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;

        public NotifyServiceCallCreatedHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(NotifyServiceCallCreated message)
        {
            using (_database)
            {
                var serviceCall = _database.SingleOrDefaultById<ServiceCall>(message.ServiceCallId);
                if (serviceCall == null)
                    return; 

                Employee employee = null;
                if (serviceCall.WarrantyRepresentativeEmployeeId != null)
                {
                    employee = _database.SingleById<Employee>(serviceCall.WarrantyRepresentativeEmployeeId);
                }
                var job = _database.SingleById<Job>(serviceCall.JobId);

                var serviceCallLineItems = _database.Fetch<ServiceCallLineItem>().Where(x => x.ServiceCallId == serviceCall.ServiceCallId);
                _bus.Publish<ServiceCallCreated>(x =>
                {
                    x.ServiceCallId = serviceCall.ServiceCallId;
                    x.ServiceCallNumber = serviceCall.ServiceCallNumber;
                    x.ServiceCallType = serviceCall.ServiceCallType;
                    x.WorkSummary = serviceCall.WorkSummary;
                    x.ServiceCallStatus = serviceCall.ServiceCallStatus.DisplayName;
                    x.Contact = serviceCall.Contact;
                    x.EmployeeName = employee != null ? employee.Name : null;
                    x.EmployeeNumber = employee != null ? employee.Number : null;
                    x.JobNumber = job.JobNumber;
                    x.ServiceCallLineItems = serviceCallLineItems.Select(y => new ServiceCallCreated.ServiceCallLineItem
                        {
                            ServiceCallId = serviceCall.ServiceCallId,
                            ServiceCallLineItemId = y.ServiceCallLineItemId,
                            CauseDescription = y.CauseDescription,
                            LineNumber = y.LineNumber,
                            ProblemCode = y.ProblemCode,
                            ProblemDescription = y.ProblemDescription,
                            ServiceCallLineItemStatus = y.ServiceCallLineItemStatus.DisplayName,
                            ProblemJdeCode = y.ProblemJdeCode,
                            RootCause = y.RootCause,
                            RootProblem = y.RootProblem
                        }).ToList();
                });
            }
        }
    }
}
