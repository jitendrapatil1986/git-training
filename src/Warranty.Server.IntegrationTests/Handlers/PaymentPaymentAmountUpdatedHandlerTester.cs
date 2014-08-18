using Accounting.Events.Payment;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers
{
    [TestFixture]
    public class PaymentPaymentAmountUpdatedHandlerTester : HandlerTester<PaymentPaymentAmountUpdated>
    {
        private Payment _payment;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _payment = GetSaved<Payment>();

            Send(x =>
            {
                x.JDEId = _payment.JdeIdentifier;
                x.PaymentAmount = 999.99M;
            });
        }

        [Test]
        public void Payment_Amount_Should_Be_Updated()
        {
            var payment = Get<Payment>(_payment.PaymentId);
            payment.Amount.ShouldEqual(Event.PaymentAmount);
        }
    }
}
