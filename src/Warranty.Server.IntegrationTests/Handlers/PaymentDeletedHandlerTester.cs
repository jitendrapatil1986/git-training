using System.Linq;
using Accounting.Events.Payment;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers
{
    [TestFixture]
    public class PaymentDeletedHandlerTester : HandlerTester<PaymentDeleted>
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var payment = GetSaved<Payment>();

            Send(x =>
            {
                x.JDEId = payment.JdeIdentifier;
            });
        }

        [Test]
        public void Payment_Should_Be_Deleted()
        {
            using (TestDatabase)
            {
                var payment = TestDatabase.FetchBy<Payment>(sql => sql.Where(p => p.JdeIdentifier == Event.JDEId)).FirstOrDefault();
                payment.ShouldBeNull();
            }
        }
    }
}