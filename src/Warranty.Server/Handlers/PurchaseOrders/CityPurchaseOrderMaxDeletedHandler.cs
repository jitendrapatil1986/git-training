namespace Warranty.Server.Handlers.PurchaseOrders
{
    using Accounting.Events.PurchaseOrders;
    using Core.Entities;
    using NPoco;
    using NServiceBus;

    public class CityPurchaseOrderMaxDeletedHandler : IHandleMessages<CityPurchaseOrderMaxDeleted>
    {
        private readonly IDatabase _database;

        public CityPurchaseOrderMaxDeletedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(CityPurchaseOrderMaxDeleted message)
        {
            using (_database)
            {
                var existingCity = _database.Single<City>("SELECT * FROM Cities WHERE CityCode = @0", message.JDEId);

                existingCity.PurchaseOrderMaxAmount = 0;
                _database.Update(existingCity);    
            }
        }
    }
}