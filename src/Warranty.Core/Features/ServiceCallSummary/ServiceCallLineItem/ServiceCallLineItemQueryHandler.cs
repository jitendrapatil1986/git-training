namespace Warranty.Core.Features.ServiceCallSummary.ServiceCallLineItem
{
    using System;
    using System.Collections.Generic;
    using Enumerations;
    using NPoco;
    using Security;

    public class ServiceCallLineItemQueryHandler : IQueryHandler<ServiceCallLineItemQuery, ServiceCallLineItemModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public ServiceCallLineItemQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public ServiceCallLineItemModel Handle(ServiceCallLineItemQuery query)
        {
            var user = _userSession.GetCurrentUser();
            var model = GetServiceCallLineItem(query.ServiceCallLineItemId);

            model.ServiceCallLineItemNotes = GetServiceCallLineNotes(query.ServiceCallLineItemId);
            model.ServiceCallLineItemAttachments = GetServiceCallLineAttachments(query.ServiceCallLineItemId);
            model.ProblemCodes = SharedQueries.ProblemCodes.GetProblemCodeList(_database);
            model.ServiceCallLineItemPayments = GetServiceCallLinePayments(query.ServiceCallLineItemId);
            model.ServiceCallLineItemPurchaseOrders = GetServiceCallLinePurchaseOrders(query.ServiceCallLineItemId);
            model.CanReopenLines = user.IsInRole(UserRoles.WarrantyServiceManager) || user.IsInRole(UserRoles.WarrantyServiceCoordinator);
            model.CanTakeActionOnPayments = user.IsInRole(UserRoles.WarrantyServiceManager);

            return model;
        }

        private ServiceCallLineItemModel GetServiceCallLineItem(Guid serviceCallLineItemId)
        {
            const string sql = @"SELECT li.[ServiceCallLineItemId],
                                    li.[ServiceCallId],
                                    sc.[ServiceCallNumber],
                                    li.[LineNumber],
                                    li.[ProblemCode],
                                    li.[ProblemDescription],
                                    li.[CauseDescription],
                                    li.[ClassificationNote],
                                    li.[LineItemRoot],
                                    li.[CreatedDate],
                                    li.[ServiceCallLineItemStatusId] as ServiceCallLineItemStatus,
                                    li.[RootCause],
                                    li.[ProblemJdeCode],
                                    li.[ProblemDetailCode],
                                    cc.[CostCode],
                                    job.[JobNumber]
                                FROM ServiceCallLineItems li
                                INNER JOIN ServiceCalls sc
                                    ON li.ServiceCallId = sc.ServiceCallId
                                INNER JOIN Jobs job
                                    ON job.JobId = sc.JobId
                                INNER JOIN Communities community
                                    ON community.CommunityId = job.CommunityId
                                INNER JOIN Cities city
                                    ON city.CityId = community.CityId
                                LEFT JOIN CityCodeProblemCodeCostCodes cc
                                    ON cc.CityCode = city.CityCode AND cc.ProblemJdeCode = li.ProblemJDECode
                                WHERE ServiceCallLineItemId = @0";

            var result = _database.Single<ServiceCallLineItemModel>(sql, serviceCallLineItemId);

            return result;
        }

        private IEnumerable<ServiceCallLineItemModel.ServiceCallLineItemAttachment> GetServiceCallLineAttachments(Guid serviceCallLineItemId)
        {
            const string sql = @"SELECT [ServiceCallAttachmentId]
                                        ,[ServiceCallLineItemId]
                                        ,[DisplayName]
                                        ,[CreatedDate]
                                        ,[CreatedBy]
                                FROM [ServiceCallAttachments]
                                WHERE ServiceCallLineItemId = @0 AND IsDeleted=0";

            var result = _database.Fetch<ServiceCallLineItemModel.ServiceCallLineItemAttachment>(sql, serviceCallLineItemId.ToString());

            return result;
        }

        private IEnumerable<ServiceCallLineItemModel.ServiceCallLineItemPayment> GetServiceCallLinePayments(Guid serviceCallLineItemId)
        {
            const string sql = @"SELECT 
                                    p.PaymentId
                                    , p.VendorNumber
                                    , p.VendorName
                                    , p.Amount
                                    , p.PaymentStatus
                                    , p.InvoiceNumber
                                    , p.ServiceCallLineItemId
                                    , p.CreatedDate as PaymentCreatedDate
                                    , p.HoldComments
                                    , p.HoldDate
                                    , b.backchargeId
                                    , b.BackchargeVendorNumber
                                    , b.BackchargeVendorName
                                    , b.BackchargeReason
                                    , b.BackchargeAmount
                                    , b.PersonNotified
                                    , b.PersonNotifiedPhoneNumber
                                    , b.PersonNotifiedDate
                                    , b.BackchargeResponseFromVendor
                                    , b.BackchargeStatus
                                    , b.HoldComments backchargeHoldComments
                                    , b.HoldDate backchargeHoldDate
                                    , b.DenyComments backchargeDenyComments
                                    , b.DenyDate backchargeDenyDate
                                    , CASE WHEN b.BackchargeVendorNumber IS NOT NULL THEN 1 ELSE 0 END AS IsBackcharge
                                FROM payments p
                                    LEFT JOIN backcharges b
                                       ON p.PaymentId = b.PaymentId
                                    INNER JOIN ServiceCallLineItems scli
                                       ON scli.ServiceCallLineItemId = p.ServiceCallLineItemId
                                WHERE p.ServiceCallLineItemId = @0 ORDER BY p.CreatedDate desc";

            var result = _database.Fetch<ServiceCallLineItemModel.ServiceCallLineItemPayment>(sql, serviceCallLineItemId);

            return result;
        }

        private IEnumerable<ServiceCallLineItemModel.ServiceCallLineItemNote> GetServiceCallLineNotes(Guid serviceCallLineItemId)
        {
            const string sql = @"SELECT [ServiceCallNoteId]
                                      ,[ServiceCallId]
                                      ,[ServiceCallNote] as Note
                                      ,[ServiceCallLineItemId]
                                      ,[CreatedDate]
                                      ,[CreatedBy]
                                FROM [ServiceCallNotes]
                                WHERE ServiceCallLineItemId = @0";

            var result = _database.Fetch<ServiceCallLineItemModel.ServiceCallLineItemNote>(sql, serviceCallLineItemId.ToString());

            return result;
        }

        private IEnumerable<ServiceCallLineItemModel.ServiceCallLineItemPurchaseOrder> GetServiceCallLinePurchaseOrders(Guid serviceCallLineItemId)
        {
            const string sql = @"SELECT p.[PurchaseOrderId]
                                    ,[PurchaseOrderNumber]
                                    ,[VendorNumber]
                                    ,[VendorName]
                                    ,p.[CreatedDate]
                                    ,[ServiceCallLineItemId]
                                    ,[JobNumber]
                                    ,[LineNumber]
                                    ,[Quantity]
                                    ,[UnitCost]
                                    ,[PurchaseOrderLineItemStatusId] as PurchaseOrderLineItemStatus
                                FROM [PurchaseOrders] p
                                INNER JOIN PurchaseOrderLineItems l
                                ON p.PurchaseOrderId = l.PurchaseOrderId
                                WHERE p.ServiceCallLineItemId = @0";

            var result = _database.FetchOneToMany<ServiceCallLineItemModel.ServiceCallLineItemPurchaseOrder,ServiceCallLineItemModel.ServiceCallLineItemPurchaseOrderLine>(x => x.PurchaseOrderId, sql, serviceCallLineItemId);

            return result;
        }
    }
}