using Accounting.Events.Payment;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers.Payments
{
    using Core.Enumerations;

    [TestFixture]
    public class PaymentStatusUpdatedHandlerTester : HandlerTester<PaymentStatusUpdated>
    {
        private Payment _payment;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _payment = GetSaved<Payment>();

            Send(x =>
            {
                x.JDEId = _payment.JdeIdentifier;
                x.Status = "A";
            });
        }

        [Test]
        public void Payment_Status_Should_Be_Updated()
        {
            var payment = Get<Payment>(_payment.PaymentId);
            payment.PaymentStatus.ShouldEqual(PaymentStatus.FromJdeCode(Event.Status));
        }
    }
}
