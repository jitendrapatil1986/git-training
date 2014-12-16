using System;
using NPoco;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;

namespace Warranty.Server.IntegrationTests.SetUp
{
    public class PaymentEntityBuilder : EntityBuilder<Payment>
    {
        public PaymentEntityBuilder(IDatabase database) : base(database)
        {
        }

        public override Payment GetSaved(Action<Payment> action)
        {
            var id = Guid.NewGuid();

            var entity = new Payment{
                JdeIdentifier = id.ToString("n"),
                PaymentId = id,
                Amount = 1,
                CreatedBy = "test",
                CreatedDate = DateTime.UtcNow,
                PaymentStatus = PaymentStatus.Approved,
            };

            return Saved(entity, action);
        }
    }
}