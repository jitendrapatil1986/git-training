namespace Warranty.Server.Handlers.Communities
{
    using Accounting.Events;
    using Core.Entities;
    using NPoco;
    using NServiceBus;

    public class DivisionNameUpdatedHandler : IHandleMessages<DivisionNameUpdated>
    {
        private readonly IDatabase _database;

        public DivisionNameUpdatedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(DivisionNameUpdated message)
        {
            var division = _database.SingleOrDefault<Division>("WHERE DivisionCode=@0", message.JDEId);
            if (division == null)
                return;

            division.DivisionName = message.Name;
            _database.Save<Division>(division);
        }
    }
}
