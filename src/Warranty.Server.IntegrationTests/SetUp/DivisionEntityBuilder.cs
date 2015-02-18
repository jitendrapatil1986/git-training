using System;
using NPoco;
using Warranty.Core.Entities;

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
            var entity = new Division
            {
                DivisionCode = "DIV",
                CreatedBy = "test",
                CreatedDate = DateTime.UtcNow,
            };

            return Saved(entity, action);
        }
    }
}