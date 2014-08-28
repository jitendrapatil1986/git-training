using System;
using NPoco;
using Warranty.Core.ApprovalInfrastructure.Interfaces;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;

namespace Warranty.Core.ApprovalInfrastructure.ApprovalServices
{
    public class ServiceCallApprovalService : IApprovalService<ServiceCall>
    {
        private readonly IDatabase _database;

        public ServiceCallApprovalService(IDatabase database)
        {
            _database = database;
        }

        public ServiceCall Approve(Guid id)
        {
            return UpdateServiceCall(id, ServiceCallStatus.Open);
        }

        public ServiceCall Deny(Guid id)
        {
            return UpdateServiceCall(id, ServiceCallStatus.Closed);
        }

        private ServiceCall UpdateServiceCall(Guid id, ServiceCallStatus newStatus)
        {
            using (_database)
            {
                var serviceCall = _database.SingleById<ServiceCall>(id);
                serviceCall.ServiceCallStatus = newStatus;
                _database.Update(serviceCall);
                return serviceCall;
            }
        }
    }
}
