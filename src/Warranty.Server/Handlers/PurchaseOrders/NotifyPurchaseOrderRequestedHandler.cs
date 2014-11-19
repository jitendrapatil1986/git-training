namespace Warranty.Server.Handlers.PurchaseOrders
{
    using System.Collections.Generic;
    using Accounting.Commands.PurchaseOrders;
    using Core;
    using Core.Entities;
    using InnerMessages;
    using NPoco;
    using NServiceBus;
    using System.Linq;

    public class NotifyPurchaseOrderRequestedHandler : IHandleMessages<NotifyPurchaseOrderRequested>
    {
        private readonly IBus _bus;
        private readonly IDatabase _database;

        public NotifyPurchaseOrderRequestedHandler(IBus bus, IDatabase database)
        {
            _bus = bus;
            _database = database;
        }

        public void Handle(NotifyPurchaseOrderRequested message)
        {
            using (_database)
            {
                const string sql = @"SELECT c.CityCode, cm.CommunityNumber
                                     FROM Jobs j
                                     INNER JOIN Communities cm
                                     ON j.CommunityId = cm.CommunityId
                                     INNER JOIN Cities c
                                     ON c.CityId = cm.CityId
                                     WHERE j.JobNumber = @0";

                var model = _database.Single<PurchaseOrderRequestedModel>(sql, message.JobNumber);

                model.PurchaseOrder = _database.SingleById<PurchaseOrder>(message.PurchaseOrderId);

                if (model.PurchaseOrder == null)
                    return;

                model.PurchaseOrderLineItems = _database.Fetch<PurchaseOrderLineItem>().Where(x => x.PurchaseOrderId == model.PurchaseOrder.PurchaseOrderId);

                var purchaseOrderRequest = new RequestPurchaseOrder
                    {
                        PurchaseOrderIdentifier = model.PurchaseOrder.PurchaseOrderId.ToString(),
                        CostCode = model.PurchaseOrder.CostCode,
                        VendorNumber = model.PurchaseOrder.VendorNumber,
                        DeliveryInstructions = model.PurchaseOrder.DeliveryInstructions.JdeCode,
                        VendorNotes = model.PurchaseOrder.PurchaseOrderNote,
                        CostCenter = model.PurchaseOrder.JobNumber,
                        Market = model.CityCode,
                        CommunityNumber = model.CommunityNumber,
                        ObjectAccount = model.PurchaseOrder.ObjectAccount,
                        IsCommunityLevel = false,
                        CopyToBuilder = false,
                        RequestedDate = model.PurchaseOrder.DeliveryDate ?? SystemTime.Now.Date,
                        UserId = message.LoginName,
                        LineItems = model.PurchaseOrderLineItems.Select(y => new RequestPurchaseOrder.LineItem
                            {
                                Quantity = y.Quantity,
                                UnitPrice = y.UnitCost,
                                ItemDescription = y.Description,
                            })
                    };

                _bus.Send(purchaseOrderRequest);
            }
        }

        public class PurchaseOrderRequestedModel
        {
            public string CommunityNumber { get; set; }
            public string CityCode { get; set; }
            public PurchaseOrder PurchaseOrder { get; set; }
            public IEnumerable<PurchaseOrderLineItem> PurchaseOrderLineItems { get; set; }
        }
    }
}