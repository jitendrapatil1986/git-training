namespace Warranty.IntegrationTests.PersistenceTests
{
    using NUnit.Framework;
    using Should;
    using Core.Entities;
    using Tests.Core;

    [TestFixture]
    public class PaymentMapTester : PersistenceTesterBase
    {
        private Payment _payment;

        [SetUp]
        public void TestFixtureSetup()
        {
            _payment = new Payment
                           {
                               Amount = .12M,
                               JdeIdentifier = "123",
                               JobNumber = "12",
                               PaymentStatus = "A",
                               VendorNumber = "1"
                           };

            Insert(_payment);
        }

        [Test]
        public void Payment_Should_Be_Inserted_With_Values()
        {
            var persistedPayment = Load<Payment>(_payment.PaymentId);
            var userName = new TestWarrantyUserSession().GetCurrentUser().UserName;

            persistedPayment.ShouldNotBeNull();
            persistedPayment.Amount.ShouldEqual(_payment.Amount);
            persistedPayment.JobNumber.ShouldEqual(_payment.JobNumber);
            persistedPayment.PaymentStatus.ShouldEqual(_payment.PaymentStatus);
            persistedPayment.VendorNumber.ShouldEqual(_payment.VendorNumber);

            persistedPayment.CreatedDate.HasValue.ShouldBeTrue();
            persistedPayment.CreatedBy.ShouldEqual(userName);
        }

        [Test]
        public void Payment_Should_Be_Updated_With_Values()
        {
            _payment.Amount = 30;
            _payment.JobNumber = "NEWNMBER";
            Update(_payment);

            var persistedPayment = Load<Payment>(_payment.PaymentId);
            var userName = new TestWarrantyUserSession().GetCurrentUser().UserName;

            persistedPayment.ShouldNotBeNull();
            persistedPayment.Amount.ShouldEqual(_payment.Amount);
            persistedPayment.JobNumber.ShouldEqual(_payment.JobNumber);
            persistedPayment.PaymentStatus.ShouldEqual(_payment.PaymentStatus);
            persistedPayment.VendorNumber.ShouldEqual(_payment.VendorNumber);

            persistedPayment.CreatedDate.HasValue.ShouldBeTrue();
            persistedPayment.CreatedBy.ShouldEqual(userName);

            persistedPayment.UpdatedDate.HasValue.ShouldBeTrue();
            persistedPayment.UpdatedBy.ShouldEqual(userName);
        }
    }
}
