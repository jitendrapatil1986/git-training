namespace Warranty.Core.Features.ServiceCallPurchaseOrderDetail
{
    using Configurations;
    using NPoco;
    using System;
    using System.Linq;

    public class ServiceCallPurchaseOrderDetailQueryHandler : IQueryHandler<ServiceCallPurchaseOrderDetailQuery, ServiceCallPurchaseOrderDetailModel>
    {
        private readonly IDatabase _database;

        public ServiceCallPurchaseOrderDetailQueryHandler(IDatabase database)
        {
            _database = database;
        }

        public ServiceCallPurchaseOrderDetailModel Handle(ServiceCallPurchaseOrderDetailQuery query)
        {
            const string sql = @"SELECT j.JobId, j.JobNumber, j.AddressLine, j.City, j.StateCode, j.PostalCode, sc.ServiceCallNumber, l.ServiceCallLineItemId, c.PurchaseOrderMaxAmount, c.CityCode
                                FROM Jobs j
                                INNER JOIN ServiceCalls sc
                                ON j.JobId = sc.JobId
                                INNER JOIN ServiceCallLineItems l
                                ON sc.ServiceCallId = l.ServiceCallId
                                INNER JOIN Communities com 
                                ON com.CommunityId = j.CommunityId
                                INNER JOIN Cities c 
                                ON c.CityId = com.CityId
                                WHERE l.ServiceCallLineItemId = @0";

            var result = _database.SingleOrDefault<ServiceCallPurchaseOrderDetailModel>(sql, query.ServiceCallLineItemId);

            result.ServiceCallLineItemPurchaseOrders = GetServiceCallLinePurchaseOrder(query.ServiceCallLineItemId, query.PurchaseOrderId);

            return result;
        }

        private ServiceCallPurchaseOrderDetailModel.ServiceCallLineItemPurchaseOrder GetServiceCallLinePurchaseOrder(Guid serviceCallLineItemId, Guid PurchaseOrderId)
        {
            const string sql = @"SELECT p.[PurchaseOrderId]
                                    ,[PurchaseOrderNumber]
                                    ,[CostCode] 
                                    ,[VendorNumber]
                                    ,[VendorName]
                                    ,p.[DeliveryInstructions]
                                    ,p.[CreatedDate]
                                    ,p.[ObjectAccount]
                                    ,p.[PurchaseOrderNote]
                                    ,[ServiceCallLineItemId]
                                    ,[JobNumber]
                                    ,[LineNumber]
                                    ,[Quantity]
                                    ,[UnitCost]
                                    ,[Description]
                                    ,[PurchaseOrderLineItemStatusId] as PurchaseOrderLineItemStatus
                                FROM [PurchaseOrders] p
                                INNER JOIN PurchaseOrderLineItems l
                                ON p.PurchaseOrderId = l.PurchaseOrderId
                                WHERE p.ServiceCallLineItemId = @0 AND p.PurchaseOrderId = @1";

            var result = _database.FetchOneToMany<ServiceCallPurchaseOrderDetailModel.ServiceCallLineItemPurchaseOrder, ServiceCallPurchaseOrderDetailModel.ServiceCallLineItemPurchaseOrderLine>(x => x.PurchaseOrderId, sql, serviceCallLineItemId, PurchaseOrderId).FirstOrDefault();

            if (!string.IsNullOrEmpty(result.ObjectAccount))
            {
                result.IsMaterialObjectAccount = WarrantyConstants.MaterialObjectAccounts.Contains(result.ObjectAccount) ? true : false;
            }
            return result;
        }
    }
}