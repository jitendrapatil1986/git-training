namespace Warranty.Core.Features.ServiceCallPurchaseOrderSearch
{
    public class ServiceCallPurchaseOrderSearchQuery : IQuery<ServiceCallPurchaseOrderSearchModel>
    {
        public ServiceCallPurchaseOrderSearchModel queryModel { get; set; }

        public ServiceCallPurchaseOrderSearchQuery()
        {
            queryModel = new ServiceCallPurchaseOrderSearchModel();
        }
    }
}
