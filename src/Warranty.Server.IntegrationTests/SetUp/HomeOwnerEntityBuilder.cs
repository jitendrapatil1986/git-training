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
            var r = new Random();

            var entity = new HomeOwner
            {
                HomeOwnerNumber = r.Next(100),
                HomeOwnerName = "Goodman"
            };

            return Saved(entity, action);
        }
    }
}