namespace Warranty.Core.Features.ManageLookups
{
    using System;
    using Entities.Lookups;
    using NPoco;

    public class CreateLookupItemCommandHandler : ICommandHandler<CreateLookupItemCommand, int>
    {
        private readonly IDatabase _database;

        public CreateLookupItemCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public int Handle(CreateLookupItemCommand message)
        {
            if (string.IsNullOrEmpty(message.DisplayName))
                return 0;

            var lookupType = LookupEntity.GetTypeFromName(message.LookupType);
            var newLookupClass = Activator.CreateInstance(lookupType) as LookupEntity;

            newLookupClass.DisplayName = message.DisplayName;
           _database.Insert(newLookupClass);
            return newLookupClass.Id;
        }
    }
}
