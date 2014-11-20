namespace Warranty.Server.Handlers.PurchaseOrders
{
    using Accounting.Events.PurchaseOrders;
    using Core.Entities;
    using NPoco;
    using NServiceBus;

    public class CityPurchaseOrderMaxAddedHandler : IHandleMessages<CityPurchaseOrderMaxAdded>
    {
        private readonly IDatabase _database;

        public CityPurchaseOrderMaxAddedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(CityPurchaseOrderMaxAdded message)
        {
            using (_database)
            {
                var existingCity = _database.SingleOrDefault<City>("SELECT * FROM Cities WHERE CityCode = @0", message.JDEId);
                if (existingCity == null)
                {
                    var city = new City()
                    {
                        CityCode = message.JDEId,
                        CityName = message.JDEId,
                        PurchaseOrderMaxAmount = message.MaxPurchaseOrderAmount
                    };
                    _database.Insert(city);
                }
                else
                {
                    existingCity.PurchaseOrderMaxAmount = message.MaxPurchaseOrderAmount;
                    _database.Update(existingCity);
                }
            }
        }
    }
}
