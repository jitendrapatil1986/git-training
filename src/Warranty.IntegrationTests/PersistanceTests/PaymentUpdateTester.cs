using System;
using NUnit.Framework;
using Should;
using Warranty.Core.DataAccess;
using Warranty.Core.Entities;
using Warranty.Core.Security;
using Warranty.IntegrationTests.Security;

namespace Warranty.IntegrationTests.PersistanceTests
{
    [TestFixture]
    public class PaymentUpdateTester : PersistanceBaseTester
    {
        private readonly Payment payment = new Payment
        {
            PaymentId = GuidComb.Generate(),
            Amount = .12M,
            JdeIdentifier = "123",
            JobNumber = "12",
            PaymentStatus = "A",
            VendorNumber = "1"
        };

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            using (TestDatabase)
            {
                TestDatabase.Insert(payment);

                payment.Amount = 9999.99M;
                TestDatabase.Update(payment);
            }
        }

        [Test]
        public void Payment_Should_Be_Updated_With_Values()
        {
            using (TestDatabase)
            {
                var persistedPayment = TestDatabase.SingleById<Payment>(payment.PaymentId);
                var userName = new WarrantyUserSession().GetCurrentUser().UserName;

                persistedPayment.ShouldNotBeNull();
                persistedPayment.Amount.ShouldEqual(payment.Amount);
                persistedPayment.JobNumber.ShouldEqual(payment.JobNumber);
                persistedPayment.PaymentStatus.ShouldEqual(payment.PaymentStatus);
                persistedPayment.VendorNumber.ShouldEqual(payment.VendorNumber);

                persistedPayment.CreatedDate.HasValue.ShouldBeTrue();
                persistedPayment.CreatedBy.ShouldEqual(userName);

                persistedPayment.UpdatedDate.HasValue.ShouldBeTrue();
                persistedPayment.UpdatedBy.ShouldEqual(userName);
            }
        }
    }
}