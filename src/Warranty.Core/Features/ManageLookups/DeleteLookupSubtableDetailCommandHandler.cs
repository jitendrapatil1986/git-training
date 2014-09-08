namespace Warranty.Core.Features.ManageLookups
{
    using Entities.Lookups;
    using NPoco;

    public class DeleteLookupSubtableDetailCommandHandler : ICommandHandler<DeleteLookupSubtableDetailCommand, bool>
    {
        private readonly IDatabase _database;

        public DeleteLookupSubtableDetailCommandHandler(IDatabase database)
        {
            _database = database;
        }
        public bool Handle(DeleteLookupSubtableDetailCommand message)
        {
            var lookupType = LookupEntity.GetTypeFromName(message.Model.LookupType);
            var singleByIdMethod = typeof (Database).GetMethod("SingleById");
            var genericSingleByIdMethod = singleByIdMethod.MakeGenericMethod(lookupType);

            object[] args = {message.Model.Id};

            var result = genericSingleByIdMethod.Invoke(_database, args);
            _database.Delete(result);

            return true;
        }
    }
}
