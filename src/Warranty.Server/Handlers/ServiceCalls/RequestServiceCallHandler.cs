namespace Warranty.Server.Handlers.ServiceCalls
{
    using System;
    using Commands;
    using Core.Entities;
    using Core.Enumerations;
    using Core.Services;
    using Events;
    using NPoco;
    using NServiceBus;

    public class RequestServiceCallHandler : IHandleMessages<RequestServiceCall>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;
        private readonly IServiceCallCreateService _serviceCallCreateService;

        public RequestServiceCallHandler(IBus bus, IDatabase database, IServiceCallCreateService serviceCallCreateService)
        {
            _bus = bus;
            _database = database;
            _serviceCallCreateService = serviceCallCreateService;
        }

        public void Handle(RequestServiceCall message)
        {
            using (_database)
            {
                var jobId = _database.Single<Guid>("SELECT JobId FROM Jobs WHERE JobNumber=@0", message.JobNumber);

                var serviceCallId = _serviceCallCreateService.Create(jobId, RequestType.WarrantyRequest, ServiceCallStatus.Requested);

                foreach (var line in message.LineItems)
                {
                    var serviceCallLine = new ServiceCallLineItem
                    {
                        ServiceCallId = serviceCallId,
                        LineNumber = line.LineNumber,
                        ProblemDescription = line.ProblemDescription,
                        ServiceCallLineItemStatus = ServiceCallLineItemStatus.Open,
                    };

                    _database.Insert(serviceCallLine);
                }

                _bus.Reply(new RequestServiceCallResponse
                               {
                                   LocalId = message.LocalId,
                                   ServiceCallId = serviceCallId
                               });
            }
        }
    }
}
