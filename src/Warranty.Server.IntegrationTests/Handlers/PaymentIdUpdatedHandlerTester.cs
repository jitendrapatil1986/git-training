using System.Linq;
using Accounting.Events.Payment;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers
{
    [TestFixture]
    public class PaymentIdUpdatedHandlerTester : HandlerTester<PaymentIdUpdated>
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var payment = GetSaved<Payment>();

            Send(x =>
            {
                x.New_JDEId = "456";
                x.Old_JDEId = payment.JdeIdentifier;
            });
        }

        [Test]
        public void Payment_Id_Should_Update()
        {
            using (TestDatabase)
            {
                var payment = TestDatabase.FetchBy<Payment>(sql => sql.Where(p => p.JdeIdentifier == Event.New_JDEId));
                payment.Any().ShouldBeTrue();

                payment = TestDatabase.FetchBy<Payment>(sql => sql.Where(p => p.JdeIdentifier == Event.Old_JDEId));
                payment.Any().ShouldBeFalse();
            }
        }
    }
}