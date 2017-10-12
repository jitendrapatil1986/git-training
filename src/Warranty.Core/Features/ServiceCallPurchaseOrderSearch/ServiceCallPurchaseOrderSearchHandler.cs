namespace Warranty.Core.Features.ServiceCallPurchaseOrderSearch
{
    using Common.Extensions;
    using Common.Security.Session;
    using NPoco;
    using Services;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class ServiceCallPurchaseOrderSearchHandler : IQueryHandler<ServiceCallPurchaseOrderSearchQuery, ServiceCallPurchaseOrderSearchModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public ServiceCallPurchaseOrderSearchHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public ServiceCallPurchaseOrderSearchModel Handle(ServiceCallPurchaseOrderSearchQuery query)
        {
            var currentuser = _userSession.GetCurrentUser();
            var markets = currentuser.Markets;
            using (_database)
            {
                var queryModel = query.QueryModel;
                if (!queryModel.HasSearchCriteria())
                    return queryModel;

                if((!queryModel.FromDate.HasValue && queryModel.ThruDate.HasValue) || (queryModel.FromDate.HasValue && !queryModel.ThruDate.HasValue))
                    return queryModel;

                var sql = @";with li as (
                            select li.[PurchaseOrderId]
                            from PurchaseOrderLineItems as li 
                                inner join PurchaseOrders as po on po.[PurchaseOrderId] = li.[PurchaseOrderId]
                            group by li.[PurchaseOrderId]
                        )
                        select top 100 po.[PurchaseOrderId]
                            ,   po.[PurchaseOrderNumber]
                            ,   po.[ServiceCallLineItemId]
                            ,   v.[Name] [VendorName]
                            ,   v.[Number] [VendorNumber]
                            ,   j.[JobId]
                            ,   j.[JobNumber]
                            ,   j.[AddressLine]
                        from PurchaseOrders as po
                            inner join li on po.[PurchaseOrderId] = li.[PurchaseOrderId]
                            left join Jobs j on j.[JobNumber] = po.[JobNumber]
                            left join Vendors as v on v.[Number] = po.[VendorNumber]
                            inner join Communities co
                            on j.CommunityId = co.CommunityId
                            inner join Cities cy
                            on co.CityId = cy.CityId
                            where((cy.CityCode IN ({0})) and
                                  (po.[PurchaseOrderNumber] = @0 or @0 IS NULL) and
                                  (po.[PurchaseOrderNumber] IS NOT NULL) and
                                  (po.[VendorNumber] = @1 or @1 IS NULL) and
                                  (po.[JobNumber] = @2 or @2 IS NULL) and
                                  ((po.[CreatedDate] between @3 and @4) or( @3 IS NULL AND @4 IS NULL))
                                  )
                        order by
                            po.[CreatedDate] desc, v.[Name], j.[JobNumber]";

                var result = _database.Fetch<ServiceCallPurchaseOrderSearchModel.PurchaseOrderDetail>(string.Format(sql, markets.CommaSeparateWrapWithSingleQuote()), queryModel.PurchaseOrderNumber, queryModel.VendorNumber, queryModel.JobNumber, queryModel.FromDate, queryModel.ThruDate);
                if (result != null)
                {
                    queryModel.Results = result;
                }
                
                return queryModel;
            }
        }
    }
}
