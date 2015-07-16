using System;

namespace Warranty.IntegrationTests.PersistenceTests
{
    using Core.Enumerations;
    using NUnit.Framework;
    using Should;
    using Core.Entities;
    using Tests.Core;

    [TestFixture]
    public class PaymentMapTester : PersistenceTesterBase
    {
        private static Payment _payment;

        [SetUp]
        public void TestFixtureSetup()
        {
            var serviceCallId = Guid.NewGuid();
            var jobId = Guid.NewGuid();
            var communityId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();
            var lineItemId = Guid.NewGuid();

            var job = new Job
            {
                AddressLine = "123 Street",
                City = "Houston",
                JobNumber = jobId.ToString().Substring(1,8),
                CloseDate = DateTime.Now,
                CommunityId = communityId,
                JobId = jobId
            };

            var community = new Community
            {
                CommunityId = communityId,
                CommunityName = "The Harbour",
                CommunityNumber = communityId.ToString().Substring(1,4),
                CityId = null,
                DivisionId = null,
                ProjectId = null,
                SateliteCityId = null
            };

            var employee = new Employee
            {
                EmployeeId = employeeId,
                Number = Guid.NewGuid().ToString().Substring(0, 8),
                Name = "Employee A"
            };

            var serviceCall = new ServiceCall
            {
                CompletionDate = null,
                Contact = "Mr. Test",
                HomeOwnerSignature = "sign",
                IsEscalated = true,
                IsSpecialProject = true,
                JobId = jobId,
                ServiceCallId = serviceCallId,
                ServiceCallStatus = ServiceCallStatus.Open,  //TODO: Need to update.
                ServiceCallNumber = 12345,
                ServiceCallType = "Warranty",  //TODO: Need to update.
                WarrantyRepresentativeEmployeeId = employeeId,
                WorkSummary = "Testing",
                HomeownerVerificationType = HomeownerVerificationType.NotVerified,
            };

            var serviceCallLineItem = new ServiceCallLineItem
            {
                ServiceCallId = serviceCallId,
                ServiceCallLineItemId = lineItemId,
                ServiceCallLineItemStatus = ServiceCallLineItemStatus.Open,
            };

            _payment = new Payment
                           {
                               Amount = .12M,
                               JdeIdentifier = "123",
                               JobNumber = "12",
                               PaymentStatus = PaymentStatus.Pending,
                               ServiceCallLineItemId = lineItemId,
                               VendorNumber = "1"
                           };

            Insert(employee);
            Insert(community);
            Insert(job);

            Insert(serviceCall);
            Insert(serviceCallLineItem);

            Insert(_payment);
        }

        [Test]
        public void Payment_Should_Be_Inserted_With_Values()
        {
            var persistedPayment = Load<Payment>(_payment.PaymentId);
            var userName = new TestWarrantyUserSession().GetActualUser().UserName;

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
            var userName = new TestWarrantyUserSession().GetActualUser().UserName;

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
