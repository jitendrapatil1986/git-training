using System.Linq;
using Accounting.Events.Payment;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers
{
    [TestFixture]
    public class PaymentPaymentAmountUpdatedHandlerTester : HandlerTester<PaymentPaymentAmountUpdated>
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var payment = GetSaved<Payment>();

            Send(x =>
            {
                x.JDEId = payment.JdeIdentifier;
                x.PaymentAmount = 999.99M;
            });
        }

        [Test]
        public void Payment_Amount_Should_Be_Updated()
        {
            using (TestDatabase)
            {
                var payment = TestDatabase.FetchBy<Payment>(sql => sql.Where(p => p.JdeIdentifier == Event.JDEId)).FirstOrDefault();
                payment.Amount.ShouldEqual(Event.PaymentAmount);
            }
        }
    }
}