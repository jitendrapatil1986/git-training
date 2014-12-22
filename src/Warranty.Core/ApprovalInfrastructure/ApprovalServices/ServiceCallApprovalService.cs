using System;
using NPoco;
using Warranty.Core.ApprovalInfrastructure.Interfaces;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;

namespace Warranty.Core.ApprovalInfrastructure.ApprovalServices
{
    using Events;
    using NServiceBus;

    public class ServiceCallApprovalService : IApprovalService<ServiceCall>
    {
        private readonly IDatabase _database;
        private readonly IBus _bus;

        public ServiceCallApprovalService(IDatabase database, IBus bus)
        {
            _database = database;
            _bus = bus;
        }

        public ServiceCall Approve(Guid id)
        {
            return UpdateServiceCall(id, ServiceCallStatus.Open);
        }

        public ServiceCall Deny(Guid id)
        {
            return UpdateServiceCall(id, ServiceCallStatus.Complete);
        }

        private ServiceCall UpdateServiceCall(Guid id, ServiceCallStatus newStatus)
        {
            using (_database)
            {
                var serviceCall = _database.SingleById<ServiceCall>(id);
                serviceCall.ServiceCallStatus = newStatus;
                _database.Update(serviceCall);
                _bus.Send<ServiceCallStatusChanged>(x =>
                {
                    x.ServiceCallId = serviceCall.ServiceCallId;
                });
                return serviceCall;
            }
        }
    }
}
