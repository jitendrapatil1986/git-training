using System.Linq;
using Accounting.Events.Payment;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers
{
    [TestFixture]
    public class PaymentAddedHandlerTester : HandlerTester<PaymentAdded>
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            Send(x =>
            {
                x.PaymentAmount = 1546.35M;
            });
        }

        [Test]
        public void Payment_Should_Be_Inserted()
        {
            using (TestDatabase)
            {
                var payment = TestDatabase.FetchBy<Payment>(sql => sql.Where(p => p.Amount == Event.PaymentAmount));
                payment.Any().ShouldBeTrue();
            }
        }
    }
}