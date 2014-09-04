﻿namespace Warranty.Core.Features.ManageLookups
{
    using System.Linq;
    using System.Reflection;
    using Entities.Lookups;
    using NPoco;

    public class DeleteLookupSubtableDetailCommandHandler : ICommandHandler<DeleteLookupSubtableDetailModel, int>
    {
        private readonly IDatabase _database;

        public DeleteLookupSubtableDetailCommandHandler(IDatabase database)
        {
            _database = database;
        }
        public int Handle(DeleteLookupSubtableDetailModel message)
        {
            var lookupType = Assembly.GetAssembly(typeof(LookupEntity)).GetTypes().Single(x => x.Name == message.LookupType);
            var singleByIdMethod = typeof (Database).GetMethod("SingleById");
            var genericSingleByIdMethod = singleByIdMethod.MakeGenericMethod(lookupType);

            object[] args = {message.Id};

            var result = genericSingleByIdMethod.Invoke(_database, args);
            _database.Delete(result);
            return message.Id;
        }
    }
}
