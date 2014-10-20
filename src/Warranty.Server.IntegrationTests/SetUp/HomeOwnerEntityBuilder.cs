using System;
using NPoco;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.SetUp
{
    public class HomeOwnerEntityBuilder : EntityBuilder<HomeOwner>
    {
        public HomeOwnerEntityBuilder(IDatabase database)
            : base(database)
        {
        }

        public override HomeOwner GetSaved(Action<HomeOwner> action)
        {
            var entity = new HomeOwner
            {
                HomeOwnerNumber = 23,
                HomeOwnerName = "Goodman"
            };

            return Saved(entity, action);
        }
    }
}