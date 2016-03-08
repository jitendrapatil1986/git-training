using System;
using NPoco;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;

namespace Warranty.IntegrationTests.MediatorMessagingTests.EntityBuilders
{
    public class PaymentEntityBuilder : EntityBuilder<Payment>
    {
        public PaymentEntityBuilder(IDatabase database) : base(database)
        {
        }

        public override Payment GetSaved(Action<Payment> action)
        {
            var entity = new Payment
            {
                PaymentStatus = PaymentStatus.Pending
            };

            return Saved(entity, action);
        }
    }
}
