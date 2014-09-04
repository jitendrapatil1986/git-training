﻿namespace Warranty.Core.Features.ManageLookups
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Entities.Lookups;
    using NPoco;

    public class CreateLookupSubtableDetailCommandHandler : ICommandHandler<CreateLookupSubtableDetailCommand, bool>
    {
        private readonly IDatabase _database;

        public CreateLookupSubtableDetailCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public bool Handle(CreateLookupSubtableDetailCommand message)
        {
            var lookupType = Assembly.GetAssembly(typeof(LookupEntity)).GetTypes().Single(x => x.Name == message.Model.LookupType);
            var newLookupClass = Activator.CreateInstance(lookupType) as LookupEntity;

            newLookupClass.DisplayName = message.Model.DisplayName;
            _database.Insert(newLookupClass);

            return true;
        }
    }
}
