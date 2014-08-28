using System;
using NPoco;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;

namespace Warranty.Core.ApprovalInfrastructure.ApprovalServices
{
    public class ServiceCallApprovalService : ApprovalService<ServiceCall>
    {
        private readonly IDatabase _database;

        public ServiceCallApprovalService(IDatabase database)
        {
            _database = database;
        }

        public override ServiceCall Approve(Guid id)
        {
            return UpdateServiceCall(id, ServiceCallStatus.Open);
        }

        public override ServiceCall Deny(Guid id)
        {
            return UpdateServiceCall(id, ServiceCallStatus.Closed);
        }

        private ServiceCall UpdateServiceCall(Guid id, ServiceCallStatus newStatus)
        {
            using (_database)
            {
                _database.Execute("Update ServiceCalls Set ServiceCallStatusId = @0 where ServiceCallId = @1", newStatus.Value, id);
                return _database.Single<ServiceCall>("Select * from ServiceCalls where ServiceCallId = @0", id);
            }
        }
    }
}
