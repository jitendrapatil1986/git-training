namespace Warranty.Core.DataAccess.Mappings.Lookups
{
    using Entities.Lookups;

    public class ServiceCallTypeMap : LookupMap<ServiceCallType>
    {
        public ServiceCallTypeMap() : base("lookups.ServiceCallTypes", "ServiceCallTypeId", "ServiceCallType")
        {
        }
    }
}