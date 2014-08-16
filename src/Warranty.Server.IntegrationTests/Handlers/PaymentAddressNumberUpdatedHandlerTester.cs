using NUnit.Framework;
using Should;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers
{
    using Accounting.Events.Payment;

    [TestFixture]
    public class PaymentAddressNumberUpdatedHandlerTester : HandlerTester<PaymentAddressNumberUpdated>
    {
        private Payment _payment;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _payment = GetSaved<Payment>();

            Send(x =>
            {
                x.JDEId = _payment.JdeIdentifier;
                x.AddressNumber = "1231111";
            });
        }

        [Test]
        public void Payment_Vendor_Number_Should_Be_Updated()
        {
            var payment = Get<Payment>(_payment.PaymentId);
            payment.VendorNumber.ShouldEqual(Event.AddressNumber);
        }
    }
}
