namespace Warranty.Core.DataAccess.Mappings.Lookups
{
    using Entities.Lookups;

    public class ServiceCallStatusMap : LookupMap<ServiceCallStatus>
    {
        public ServiceCallStatusMap() : base("lookups.ServiceCallStatuses", "ServiceCallStatusId", "ServiceCallStatus")
        {
        }
    }
}