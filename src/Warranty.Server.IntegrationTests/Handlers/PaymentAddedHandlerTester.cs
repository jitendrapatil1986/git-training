using Accounting.Events.Payment;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers
{
    using System.Linq;
    using Configuration;

    [TestFixture]
    public class PaymentAddedHandlerTester : HandlerTester<PaymentAdded>
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            Send(x =>
            {
                x.JDEId = "JDEID";
                x.ObjectAccount = WarrantyConstants.LaborObjectAccounts.First();
                x.PaymentAmount = 1546.35M;
                x.AddressNumber = "12345";
                x.Status = "X";
                x.CostCenter = "12341234";
            });
        }

        [Test]
        public void Payment_Should_Be_Inserted()
        {
            var payment = Get<Payment>(Event.JDEId);
            payment.Amount.ShouldEqual(Event.PaymentAmount);
            payment.PaymentStatus.ShouldEqual(Event.Status);
            payment.VendorNumber.ShouldEqual(Event.AddressNumber);
            payment.JobNumber.ShouldEqual(Event.CostCenter);
        }
    }
}
