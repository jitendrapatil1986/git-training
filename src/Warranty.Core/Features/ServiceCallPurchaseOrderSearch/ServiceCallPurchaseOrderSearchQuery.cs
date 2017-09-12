namespace Warranty.Core.Features.ServiceCallPurchaseOrderSearch
{
    public class ServiceCallPurchaseOrderSearchQuery : IQuery<ServiceCallPurchaseOrderSearchModel>
    {
        public ServiceCallPurchaseOrderSearchModel QueryModel { get; set; }

        public ServiceCallPurchaseOrderSearchQuery()
        {
            QueryModel = new ServiceCallPurchaseOrderSearchModel();
        }
    }
}
