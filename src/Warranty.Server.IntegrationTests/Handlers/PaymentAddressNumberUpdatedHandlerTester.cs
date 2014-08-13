using System.Linq;
using Accounting.Events.Payment;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers
{
    [TestFixture]
    public class PaymentAddressNumberUpdatedHandlerTester : HandlerTester<PaymentAddressNumberUpdated>
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var payment = GetSaved<Payment>();

            Send(x =>
            {
                x.JDEId = payment.JdeIdentifier;
                x.AddressNumber = "1231111";
            });
        }

        [Test]
        public void Payment_Vendor_Number_Should_Be_Updated()
        {
            using (TestDatabase)
            {
                var payment = TestDatabase.FetchBy<Payment>(sql => sql.Where(p => p.JdeIdentifier == Event.JDEId)).FirstOrDefault();
                payment.VendorNumber.ShouldEqual(Event.AddressNumber);
            }
        }
    }
}