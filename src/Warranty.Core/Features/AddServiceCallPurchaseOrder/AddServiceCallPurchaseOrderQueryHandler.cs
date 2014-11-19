namespace Warranty.Core.Features.AddServiceCallPurchaseOrder
{
    using NPoco;

    public class AddServiceCallPurchaseOrderQueryHandler : IQueryHandler<AddServiceCallPurchaseOrderQuery, AddServiceCallPurchaseOrderModel>
    {
        private readonly IDatabase _database;

        public AddServiceCallPurchaseOrderQueryHandler(IDatabase database)
        {
            _database = database;
        }

        public AddServiceCallPurchaseOrderModel Handle(AddServiceCallPurchaseOrderQuery query)
        {
            const string sql = @"SELECT j.JobId, j.JobNumber, j.AddressLine, j.City, j.StateCode, j.PostalCode, sc.ServiceCallId, sc.ServiceCallNumber, l.ServiceCallLineItemId 
                                FROM Jobs j
                                INNER JOIN ServiceCalls sc
                                ON j.JobId = sc.JobId
                                INNER JOIN ServiceCallLineItems l
                                ON sc.ServiceCallId = l.ServiceCallId
                                WHERE l.ServiceCallLineItemId = @0";

            var result = _database.SingleOrDefault<AddServiceCallPurchaseOrderModel>(sql, query.ServiceCallLineItemId);

            return result;
        }
    }
}