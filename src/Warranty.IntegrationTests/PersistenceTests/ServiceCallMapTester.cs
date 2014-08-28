using System;

namespace Warranty.IntegrationTests.PersistenceTests
{
    using NUnit.Framework;
    using Should;
    using Core.Entities;
    using Core.Enumerations;
    using Tests.Core;

    [TestFixture]
    public class ServiceCallMapTester : PersistenceTesterBase
    {
        private ServiceCall _serviceCall;
        private Job _job;
        private Guid _serviceCallId;
        private Guid _jobId;
        private Guid _communityId;
        private Guid _builderId;
        private Guid _homeownerId;
        private Guid _salespersonId;
        private Guid _employeeId;
        private HomeOwner _homeOwner;
        private Community _community;
        private Employee _employee;

        [SetUp]
        public void TestFixtureSetup()
        {
            _serviceCallId = Guid.NewGuid();
            _jobId = Guid.NewGuid();
            _communityId = Guid.NewGuid();
            _builderId = Guid.NewGuid();
            _homeownerId = Guid.NewGuid();
            _salespersonId = Guid.NewGuid();
            _employeeId = Guid.NewGuid();

            _serviceCall = new ServiceCall
                {
                    CompletionDate = null,
                    Contact = "Mr. Test",
                    HomeOwnerSignature = "sign",
                    IsEscalated = true,
                    IsSpecialProject = true,
                    JobId = _jobId,
                    ServiceCallId = _serviceCallId,
                    ServiceCallStatus = ServiceCallStatus.Open,  //TODO: Need to update.
                    ServiceCallNumber = 12345,
                    ServiceCallType = "Warranty",  //TODO: Need to update.
                    WarrantyRepresentativeEmployeeId = _employeeId,
                    WorkSummary = "Testing"
                };

            _job = new Job
                {
                    AddressLine = "123 Street",
                    //BuilderEmployeeId = null, //_builderId,
                    City = "Houston",
                    CloseDate = DateTime.Now,
                    CommunityId = _communityId,
                    //CurrentHomeOwnerId = _homeownerId,
                    JobId = _jobId
                };

            _community = new Community
                {
                    CommunityId = _communityId,
                    CommunityName = "The Harbour",
                    CityId = null,
                    DivisionId = null,
                    ProjectId = null,
                    SateliteCityId = null
                };

            _employee = new Employee
                {
                    EmployeeId = _employeeId,
                    Name = "Employee A"
                };

            //_homeOwner = new HomeOwner
            //    {
            //        HomeOwnerId = _homeownerId,
            //        JobId = _jobId,
            //        HomeOwnerName = "John Doey"
            //    };

            //Insert(_homeOwner);

            Insert(_employee);
            Insert(_community);
            Insert(_job);

            Insert(_serviceCall);
        }

        [Test]
        public void ServiceCall_Should_Be_Inserted_With_Values()
        {
            var persistedServiceCall = Load<ServiceCall>(_serviceCall.ServiceCallId);
            var userName = new TestWarrantyUserSession().GetCurrentUser().UserName;

            persistedServiceCall.ShouldNotBeNull();
            persistedServiceCall.CompletionDate.ShouldEqual(_serviceCall.CompletionDate);
            persistedServiceCall.Contact.ShouldEqual(_serviceCall.Contact);
            persistedServiceCall.HomeOwnerSignature.ShouldEqual(_serviceCall.HomeOwnerSignature);
            persistedServiceCall.IsEscalated.ShouldEqual(_serviceCall.IsEscalated);
            persistedServiceCall.JobId.ShouldEqual(_serviceCall.JobId);
            persistedServiceCall.ServiceCallStatus.ShouldEqual(_serviceCall.ServiceCallStatus);
            persistedServiceCall.ServiceCallNumber.ShouldEqual(_serviceCall.ServiceCallNumber);
            persistedServiceCall.ServiceCallType.ShouldEqual(_serviceCall.ServiceCallType);
            persistedServiceCall.WarrantyRepresentativeEmployeeId.ShouldEqual(_serviceCall.WarrantyRepresentativeEmployeeId);
            persistedServiceCall.WorkSummary.ShouldEqual(_serviceCall.WorkSummary);

            persistedServiceCall.CreatedBy.ShouldEqual(userName);
            persistedServiceCall.CreatedDate.HasValue.ShouldBeTrue();
        }

        [Test]
        public void ServiceCall_Should_Be_Updated_With_Values()
        {
            _serviceCall.ServiceCallNumber = 11111;
            _serviceCall.ServiceCallType = "Mold";
            _serviceCall.Contact = "Mr. Anderson";
            Update(_serviceCall);

            var persistedServiceCall = Load<ServiceCall>(_serviceCall.ServiceCallId);
            var userName = new TestWarrantyUserSession().GetCurrentUser().UserName;

            persistedServiceCall.ShouldNotBeNull();
            persistedServiceCall.CompletionDate.ShouldEqual(_serviceCall.CompletionDate);
            persistedServiceCall.Contact.ShouldEqual(_serviceCall.Contact);
            persistedServiceCall.HomeOwnerSignature.ShouldEqual(_serviceCall.HomeOwnerSignature);
            persistedServiceCall.IsEscalated.ShouldEqual(_serviceCall.IsEscalated);
            persistedServiceCall.JobId.ShouldEqual(_serviceCall.JobId);
            persistedServiceCall.ServiceCallStatus.ShouldEqual(_serviceCall.ServiceCallStatus);
            persistedServiceCall.ServiceCallNumber.ShouldEqual(_serviceCall.ServiceCallNumber);
            persistedServiceCall.ServiceCallType.ShouldEqual(_serviceCall.ServiceCallType);
            persistedServiceCall.WarrantyRepresentativeEmployeeId.ShouldEqual(_serviceCall.WarrantyRepresentativeEmployeeId);
            persistedServiceCall.WorkSummary.ShouldEqual(_serviceCall.WorkSummary);

            persistedServiceCall.CreatedBy.ShouldEqual(userName);
            persistedServiceCall.CreatedDate.HasValue.ShouldBeTrue();

            persistedServiceCall.UpdatedBy.ShouldEqual(userName);
            persistedServiceCall.UpdatedDate.HasValue.ShouldBeTrue();
        }
    }
}
