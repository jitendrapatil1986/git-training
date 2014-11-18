using System;
using Accounting.Events.Payment;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers.Payments
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
            var payment = Get<Payment>(Event.JDEId);
            payment.VendorNumber.ShouldEqual(Event.AddressNumber);
        }
    }
}
