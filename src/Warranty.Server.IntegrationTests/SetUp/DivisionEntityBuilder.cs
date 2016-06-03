using System;
using NPoco;
using Warranty.Core.Entities;
using Warranty.Server.IntegrationTests.Handlers;

namespace Warranty.Server.IntegrationTests.SetUp
{
    public class DivisionEntityBuilder : EntityBuilder<Division>
    {
        public DivisionEntityBuilder(IDatabase database)
            : base(database)
        {
        }

        public override Division GetSaved(Action<Division> action)
        {
            var randomStringGenerator = new RandomStringGenerator();

            var entity = new Division
            {
                DivisionCode = randomStringGenerator.Get(3),
                CreatedBy = "test",
                CreatedDate = DateTime.UtcNow,
            };

            return Saved(entity, action);
        }
    }
}