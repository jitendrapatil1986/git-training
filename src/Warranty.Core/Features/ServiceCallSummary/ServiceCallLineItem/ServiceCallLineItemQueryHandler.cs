namespace Warranty.Core.Features.ServiceCallSummary.ServiceCallLineItem
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            model.CanTakeActionOnPayments = user.IsInRole(UserRoles.CustomerCareManager);
            model.Vendors = GetLineItemVendors(query.ServiceCallLineItemId);

            return model;
        }

        private IEnumerable<ServiceCallLineItemModel.Vendor> GetLineItemVendors(Guid serviceCallLineItemId)
        {
            const string sql = @"SELECT DISTINCT v.VendorId, v.Number, v.Name, ci.Value, ci.Type, jvcc.CostCode, jvcc.CostCodeDescription FROM ServiceCalls sc
                                INNER JOIN ServiceCallLineITems scli
                                    ON scli.ServiceCallId = sc.ServiceCallId 
                                INNER JOIN Jobs job
                                    ON job.JobId = sc.JobId
                                INNER JOIN Communities community
                                    ON community.CommunityId = job.CommunityId
                                INNER JOIN Cities city
                                    ON city.CityId = community.CityId
                                LEFT JOIN CityCodeProblemCodeCostCodes cc
                                    ON cc.CityCode = city.CityCode AND cc.ProblemJdeCode = scli.ProblemJDECode  
                          INNER JOIN JobVendorCostCodes jvcc
                             ON jvcc.JobId = job.JobId and jvcc.CostCode = cc.CostCode
                          INNER JOIN Vendors v
                             ON v.VendorId = jvcc.VendorId
                          INNER JOIN (SELECT VendorId, number as Value, Type as Type FROM VendorPhones
                                    UNION
                                    SELECT VendorId, email as Value, 'E-mail' as Type FROM VendorEmails) as ci 
                                    on v.vendorid = ci.vendorid
                          where scli.ServiceCallLineItemId = @0";

            var result = _database.Fetch<VendorDto>(sql, serviceCallLineItemId);

            return
                result.Select(x => new ServiceCallLineItemModel.Vendor
                {
                    Name = x.Name,
                    Number = x.Number,
                    VendorId = x.VendorId,
                    CostCodes =
                        result.Where(v => v.VendorId == x.VendorId)
                              .Select(cc => new ServiceCallLineItemModel.CostCodeModel
                              {
                                  CostCode = cc.CostCode,
                                  CostCodeDescription = cc.CostCodeDescription
                              }).Distinct().OrderBy(ob => ob.CostCode).ToList(),
                    ContactInfo =
                        result.Where(v => v.VendorId == x.VendorId)
                              .Select(cc => new ServiceCallLineItemModel.Vendor.ContactInfoModel()
                              {
                                  Value = cc.Value,
                                  Type = cc.Type
                              }).Distinct().OrderBy(ob => ob.Value).ToList()
                }).Distinct().OrderBy(x => x.Name).ToList();
        }

        public class VendorDto
        {
            public Guid VendorId { get; set; }
            public string Name { get; set; }
            public string Number { get; set; }
            public string Value { get; set; }
            public string Type { get; set; }
            public string CostCode { get; set; }
            public string CostCodeDescription { get; set; }
        }

        private ServiceCallLineItemModel GetServiceCallLineItem(Guid serviceCallLineItemId)
        {
            const string sql = @"SELECT li.[ServiceCallLineItemId],
                                    li.[ServiceCallId],
                                    sc.[ServiceCallNumber],
                                    sc.[ServiceCallType],
                                    li.[LineNumber],
                                    li.[ProblemCode],
                                    li.[ProblemDescription],
                                    li.[CauseDescription],
                                    li.[CreatedDate],
                                    li.[ServiceCallLineItemStatusId] as ServiceCallLineItemStatus,
                                    li.[RootCause],
                                    li.[ProblemJdeCode],
                                    li.[ProblemDetailCode],
                                    li.[RootCause],
                                    li.[RootProblem],
                                    cc.[CostCode],
                                    job.[JobNumber],
                                    city.CityCode
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
                                    , p.Comments
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
                                    , p.CostCode as JdeCostCode
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
                                    ,[CostCode] 
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