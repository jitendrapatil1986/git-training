using Accounting.Events.Payment;
using NUnit.Framework;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers
{
    using System;

    [TestFixture]
    public class PaymentDeletedHandlerTester : HandlerTester<PaymentDeleted>
    {
        private Payment _payment;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _payment = GetSaved<Payment>();

            Send(x =>
            {
                x.JDEId = _payment.JdeIdentifier;
            });
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void Payment_Should_Be_Deleted()
        {
            var payment = Get<Payment>(_payment.PaymentId);
        }
    }
}
