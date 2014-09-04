namespace Warranty.Core.Features.ManageLookups
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Entities.Lookups;
    using NPoco;

    public class CreateLookupSubtableDetailCommandHandler : ICommandHandler<CreateLookupSubtableDetailModel, int>
    {
        private readonly IDatabase _database;

        public CreateLookupSubtableDetailCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public int Handle(CreateLookupSubtableDetailModel message)
        {
            var lookupType = Assembly.GetAssembly(typeof(LookupEntity)).GetTypes().Single(x => x.Name == message.LookupType);
            var newLookupClass = Activator.CreateInstance(lookupType) as LookupEntity;

            newLookupClass.DisplayName = message.DisplayName;
            _database.Insert(newLookupClass);

            //TODO: Remove if not needed.
            return message.Id;
        }
    }
}
