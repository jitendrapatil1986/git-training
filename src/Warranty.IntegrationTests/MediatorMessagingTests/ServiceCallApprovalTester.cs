using System;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;
using Warranty.Core.Features.ServiceCallApproval;
using Warranty.IntegrationTests.PersistenceTests;
using Warranty.Tests.Core;

namespace Warranty.IntegrationTests.MediatorMessagingTests
{
    public class ServiceCallApprovalTester : MediatorMessagingTesterBase
    {
        private ServiceCall _serviceCall;
        private Job _job;
        private Guid _serviceCallId;
        private Guid _jobId;
        private Guid _communityId;
        private Guid _employeeId;
        private Community _community;
        private Employee _employee;

        [SetUp]
        public void TestFixtureSetup()
        {
            _serviceCallId = Guid.NewGuid();
            _jobId = Guid.NewGuid();
            _communityId = Guid.NewGuid();
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
                ServiceCallStatus = ServiceCallStatus.Requested,  //TODO: Need to update.
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

            Insert(_employee);
            Insert(_community);
            Insert(_job);
            Insert(_serviceCall);
        }

        [Test]
        public void ServiceCall_Should_Be_Approved()
        {
            var command = new ServiceCallApproveCommand
            {
                ServiceCallId = _serviceCall.ServiceCallId
            };
            Send(command);
            var affectedServiceCall = Load<ServiceCall>(_serviceCallId);
            affectedServiceCall.ServiceCallStatus.ShouldEqual(ServiceCallStatus.Open);
        }

        [Test]
        public void ServiceCall_Should_Be_Denied()
        {
            var command = new ServiceCallDenyCommand()
            {
                ServiceCallId = _serviceCall.ServiceCallId
            };
            Send(command);
            var affectedServiceCall = Load<ServiceCall>(_serviceCallId);
            affectedServiceCall.ServiceCallStatus.ShouldEqual(ServiceCallStatus.Closed);
        }
    }
}