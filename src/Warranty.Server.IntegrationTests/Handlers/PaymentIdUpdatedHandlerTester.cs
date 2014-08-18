using Accounting.Events.Payment;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers
{
    [TestFixture]
    public class PaymentIdUpdatedHandlerTester : HandlerTester<PaymentIdUpdated>
    {
        private Payment _payment;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _payment = GetSaved<Payment>();

            Send(x =>
            {
                x.New_JDEId = "456";
                x.Old_JDEId = _payment.JdeIdentifier;
            });
        }

        [Test]
        public void Payment_Id_Should_Update()
        {
            var payment = Get<Payment>(_payment.PaymentId);
            payment.JdeIdentifier.ShouldEqual(Event.New_JDEId);
        }
    }
}
