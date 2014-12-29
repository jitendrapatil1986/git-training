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
    using NServiceBus;

    public class ServiceCallApprovalTester : MediatorMessagingTesterBase
    {
        private ServiceCall _serviceCall;

        [SetUp]
        public void TestFixtureSetup()
        {
            _serviceCall = GetSaved<ServiceCall>();
        }

        [Test]
        public void ServiceCall_Should_Be_Approved()
        {
            var command = new ServiceCallApproveCommand
            {
                ServiceCallId = _serviceCall.ServiceCallId
            };
            Send(command);
            var affectedServiceCall = Load<ServiceCall>(_serviceCall.ServiceCallId);
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
            var affectedServiceCall = Load<ServiceCall>(_serviceCall.ServiceCallId);
            affectedServiceCall.ServiceCallStatus.ShouldEqual(ServiceCallStatus.Complete);
        }
    }
}