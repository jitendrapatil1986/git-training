namespace Warranty.Core.Features.AddServiceCallPurchaseOrder
{
    using System.Linq;
    using Configurations;
    using Entities;
    using Enumerations;
    using InnerMessages;
    using NPoco;
    using Common.Security.Session;
    using Services;
    using NServiceBus;
    using Extensions;

    public class AddServiceCallPurchaseOrderCommandHandler : ICommandHandler<AddServiceCallPurchaseOrderCommand>
    {
        private readonly IDatabase _database;
        private readonly IResolveObjectAccount _resolveObjectAccount;
        private readonly IBus _bus;
        private readonly IUserSession _userSession;

        public AddServiceCallPurchaseOrderCommandHandler(IDatabase database, IUserSession userSession, IResolveObjectAccount resolveObjectAccount, IBus bus)
        {
            _database = database;
            _userSession = userSession;
            _resolveObjectAccount = resolveObjectAccount;
            _bus = bus;
        }

        public void Handle(AddServiceCallPurchaseOrderCommand message)
        {
            using (_database)
            {
                var user = _userSession.GetCurrentUser();

                var job = _database.Single<Job>(@"SELECT j.* FROM Jobs j 
                                            INNER JOIN ServiceCalls sc ON sc.JobId = j.JobId
                                            INNER JOIN ServiceCallLineItems scl ON scl.ServiceCallId = sc.ServiceCallId
                                            WHERE scl.ServiceCallLineItemId = @0", message.ServiceCallLineItemId);


                var serviceCall = _database.Single<ServiceCall>(@"SELECT sc.* FROM ServiceCalls sc
                                            INNER JOIN ServiceCallLineItems scl ON scl.ServiceCallId = sc.ServiceCallId
                                            WHERE scl.ServiceCallLineItemId = @0", message.ServiceCallLineItemId);

                var rootProblem = _database.Single<string>("SELECT RootProblem FROM ServiceCallLineItems WHERE ServiceCallLineItemId=@0", message.ServiceCallLineItemId);
                var costCode = RootProblem.FromDisplayName(rootProblem).CostCode;

                var limit = _database.ExecuteScalar<decimal>(@"SELECT c.PurchaseOrderMaxAmount FROM Cities c
                                            INNER JOIN Communities co ON co.CityId = c.CityId
                                            INNER JOIN Jobs j ON j.CommunityId = co.CommunityId
                                            INNER JOIN ServiceCalls sc ON sc.JobId = j.JobId
                                            INNER JOIN ServiceCallLineItems scl ON scl.ServiceCallId = sc.ServiceCallId
                                            WHERE scl.ServiceCallLineItemId = @0", message.ServiceCallLineItemId);

                if (message.ServiceCallLineItemPurchaseOrderLines.Sum(scl => scl.Quantity*scl.UnitCost) <= limit || limit == 0)
                {
                    var purchaseOrder = new PurchaseOrder
                    {
                        CostCode = costCode,
                        DeliveryDate = message.DeliveryDate,
                        DeliveryInstructions = DeliveryInstruction.FromValue(message.DeliveryInstructions),
                        JobNumber = job.JobNumber,
                        PurchaseOrderNote = message.PurchaseOrderNote.Truncate(WarrantyConstants.DefaultJdePurchaseOrderNotesLength),
                        ServiceCallLineItemId = message.ServiceCallLineItemId,
                        VendorName = message.VendorName,
                        VendorNumber = message.VendorNumber,
                        ObjectAccount = message.IsMaterialObjectAccount
                                                  ? _resolveObjectAccount.ResolveMaterialObjectAccount(job, serviceCall)
                                                  : _resolveObjectAccount.ResolveLaborObjectAccount(job, serviceCall),
                    };

                    _database.Insert(purchaseOrder);

                    var lineNumber = 1;
                    foreach (var line in message.ServiceCallLineItemPurchaseOrderLines)
                    {
                        var purchaseOrderLineItem = new PurchaseOrderLineItem
                        {
                            Description = line.Description,
                            Quantity = line.Quantity,
                            UnitCost = line.UnitCost,
                            PurchaseOrderLineItemStatus = PurchaseOrderLineItemStatus.Open,
                            PurchaseOrderId = purchaseOrder.PurchaseOrderId,
                        };

                        if (purchaseOrderLineItem.Quantity != 0 && !string.IsNullOrEmpty(purchaseOrderLineItem.Description) && purchaseOrderLineItem.UnitCost != 0)
                        {
                            purchaseOrderLineItem.LineNumber = lineNumber;
                            purchaseOrderLineItem.Description = purchaseOrderLineItem.Description.Truncate(WarrantyConstants.DefaultJdePurchaseOrderLineItemDescriptionLength);
                            lineNumber += 1;
                            _database.Insert(purchaseOrderLineItem);
                        }
                    }

                    var community = _database.SingleOrDefaultById<Community>(job.CommunityId);
                    var communityNumber = community.CommunityNumber;

                    if (job.IsOutOfWarranty && community.CommunityStatusCode != WarrantyConstants.DefaultActiveCommunityCode)
                    {
                        communityNumber = WarrantyConfigSection.GetCity(user.Markets.FirstOrDefault()).ClosedOutCommunity;
                    }

                    _bus.Send<NotifyPurchaseOrderRequested>(x =>
                    {
                        x.PurchaseOrderId = purchaseOrder.PurchaseOrderId;
                        x.LoginName = user.LoginName;
                        x.CommunityNumber = communityNumber;
                        x.EmployeeNumber = user.EmployeeNumber;
                    });
                }
            }
        }
    }
}