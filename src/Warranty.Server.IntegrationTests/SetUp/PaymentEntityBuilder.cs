using System;
using NPoco;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.SetUp
{
    public class PaymentEntityBuilder : EntityBuilder<Payment>
    {
        public PaymentEntityBuilder(IDatabase database) : base(database)
        {
        }

        public override Payment GetSaved(Action<Payment> action)
        {
            var entity = new Payment{
                JdeIdentifier = "123",
                Amount = 1,
                CreatedBy = "test",
                CreatedDate = DateTime.UtcNow,
                PaymentId = Guid.NewGuid(),
            };

            return Saved(entity, action);
        }
    }
}