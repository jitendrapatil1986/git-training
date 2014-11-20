namespace Warranty.Server.Handlers.PurchaseOrders
{
    using System;
    using Accounting.Commands.PurchaseOrders;
    using Configuration;
    using Core;
    using Core.Entities;
    using Core.Enumerations;
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
                const string sql = @"SELECT p.*, j.JobNumber, c.CityCode, cm.CommunityNumber
                                    FROM PurchaseOrders p
                                    INNER JOIN Jobs j
                                    ON p.JobNumber = j.JobNumber
                                    INNER JOIN Communities cm
                                    ON j.CommunityId = cm.CommunityId
                                    INNER JOIN Cities c
                                    ON c.CityId = cm.CityId
                                    WHERE p.PurchaseOrderId = @0";

                var model = _database.Single<PurchaseOrderRequestedModel>(sql, message.PurchaseOrderId);

                var purchaseOrderLineItems = _database.Fetch<PurchaseOrderLineItem>().Where(x => x.PurchaseOrderId == model.PurchaseOrderId);

                var purchaseOrderRequest = new RequestPurchaseOrder
                    {
                        PurchaseOrderIdentifier = model.PurchaseOrderId.ToString(),
                        CostCode = model.CostCode,
                        VendorNumber = model.VendorNumber,
                        DeliveryInstructions = model.DeliveryInstructions.JdeCode,
                        VendorNotes = model.PurchaseOrderNote,
                        CostCenter = model.JobNumber,
                        Market = model.CityCode,
                        CommunityNumber = model.CommunityNumber,
                        ObjectAccount = model.ObjectAccount,
                        IsCommunityLevel = false,
                        CopyToBuilder = false,
                        PurchaseOrderTypeJdeIdentifier = WarrantyConstants.PurchaseOrderType,
                        RequestedDate = model.DeliveryDate ?? SystemTime.Now.Date,
                        UserId = message.LoginName,
                        LineItems = purchaseOrderLineItems.Select(y => new RequestPurchaseOrder.LineItem
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
            public Guid PurchaseOrderId { get; set; }
            public string CostCode { get; set; }
            public string VendorNumber { get; set; }
            public DeliveryInstruction DeliveryInstructions { get; set; }
            public string PurchaseOrderNote { get; set; }
            public string JobNumber { get; set; }
            public string ObjectAccount { get; set; }
            public DateTime? DeliveryDate { get; set; }
            public string CommunityNumber { get; set; }
            public string CityCode { get; set; }
        }
    }
}