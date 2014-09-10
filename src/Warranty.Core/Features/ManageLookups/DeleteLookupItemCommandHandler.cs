namespace Warranty.Core.Features.ManageLookups
{
    using Entities.Lookups;
    using NPoco;

    public class DeleteLookupItemCommandHandler : ICommandHandler<DeleteLookupItemCommand, bool>
    {
        private readonly IDatabase _database;

        public DeleteLookupItemCommandHandler(IDatabase database)
        {
            _database = database;
        }
        public bool Handle(DeleteLookupItemCommand message)
        {
            if (message.Id == 0)
                return false;

            var lookupType = LookupEntity.GetTypeFromName(message.LookupType);
            var singleByIdMethod = typeof (Database).GetMethod("SingleById");
            var genericSingleByIdMethod = singleByIdMethod.MakeGenericMethod(lookupType);

            object[] args = {message.Id};

            var result = genericSingleByIdMethod.Invoke(_database, args);
            _database.Delete(result);

            return true;
        }
    }
}
