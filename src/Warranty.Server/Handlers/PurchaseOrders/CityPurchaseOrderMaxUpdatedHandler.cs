namespace Warranty.Server.Handlers.PurchaseOrders
{
    using Accounting.Events.PurchaseOrders;
    using Core.Entities;
    using NPoco;
    using NServiceBus;

    public class CityPurchaseOrderMaxUpdatedHandler : IHandleMessages<CityPurchaseOrderMaxUpdated>
    {
        private readonly IDatabase _database;

        public CityPurchaseOrderMaxUpdatedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(CityPurchaseOrderMaxUpdated message)
        {
            using (_database)
            {
                var existingCity = _database.Single<City>("SELECT * FROM Cities WHERE CityCode = @0", message.JDEId);
                
                existingCity.PurchaseOrderMaxAmount = message.MaxPurchaseOrderAmount;
                _database.Update(existingCity);
            }
        }
    }
}