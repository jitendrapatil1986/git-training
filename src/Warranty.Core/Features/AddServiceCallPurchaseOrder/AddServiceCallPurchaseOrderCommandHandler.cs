namespace Warranty.Core.Features.AddServiceCallPurchaseOrder
{
    using Entities;
    using Enumerations;
    using InnerMessages;
    using NPoco;
    using Security;
    using Services;
    using NServiceBus;

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
                var job = _database.SingleOrDefault<Job>("SELECT JobId, CloseDate FROM Jobs WHERE JobNumber = @0", message.Model.JobNumber);
                var serviceCall = _database.SingleById<ServiceCall>(message.Model.ServiceCallId);
                
                var purchaseOrder = new PurchaseOrder
                    {
                        CostCode = WarrantyCostCode.FromValue(message.Model.CostCode).DisplayName,
                        DeliveryDate = message.Model.DeliveryDate,
                        DeliveryInstructions = DeliveryInstruction.FromValue(message.Model.DeliveryInstructions),
                        JobNumber = message.Model.JobNumber,
                        PurchaseOrderNote = message.Model.PurchaseOrderNote,
                        ServiceCallLineItemId = message.Model.ServiceCallLineItemId,
                        VendorName = message.Model.VendorName,
                        VendorNumber = message.Model.VendorNumber,
                        ObjectAccount = message.Model.IsMaterialObjectAccount
                                                  ? _resolveObjectAccount.ResolveMaterialObjectAccount(job, serviceCall)
                                                  : _resolveObjectAccount.ResolveLaborObjectAccount(job, serviceCall),
                    };

                _database.Insert(purchaseOrder);

                var lineNumber = 1;
                foreach (var line in message.Model.ServiceCallLineItemPurchaseOrderLines)
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
                        lineNumber += 1;
                        _database.Insert(purchaseOrderLineItem);
                    }
                }

                _bus.Send<NotifyPurchaseOrderRequested>(x =>
                    {
                        x.PurchaseOrderId = purchaseOrder.PurchaseOrderId;
                        x.LoginName = _userSession.GetCurrentUser().LoginName;
                        x.JobNumber = purchaseOrder.JobNumber;
                    });
            }
        }
    }
}