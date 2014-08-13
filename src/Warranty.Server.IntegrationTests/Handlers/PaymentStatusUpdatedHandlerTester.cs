using System.Linq;
using Accounting.Events.Payment;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers
{
    [TestFixture]
    public class PaymentStatusUpdatedHandlerTester : HandlerTester<PaymentStatusUpdated>
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var payment = GetSaved<Payment>();

            Send(x =>
            {
                x.JDEId = payment.JdeIdentifier;
                x.Status = "X";
            });
        }

        [Test]
        public void Payment_Status_Should_Be_Updated()
        {
            using (TestDatabase)
            {
                var payment = TestDatabase.FetchBy<Payment>(sql => sql.Where(p => p.JdeIdentifier == Event.JDEId)).FirstOrDefault();
                payment.PaymentStatus.ShouldEqual(Event.Status);
            }
        }
    }
}