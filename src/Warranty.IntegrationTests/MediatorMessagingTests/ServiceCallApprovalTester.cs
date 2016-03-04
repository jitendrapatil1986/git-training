using System;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;
using Warranty.Core.Exceptions;
using Warranty.Core.Features.ServiceCallApproval;
using Warranty.IntegrationTests.PersistenceTests;

namespace Warranty.IntegrationTests.MediatorMessagingTests
{
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
            affectedServiceCall.ShouldBeNull();
        }

        [Test, ExpectedException(typeof(DeleteServiceCallException))]
        public void ServiceCall_DenialWithPurchaseOrder_ShouldFail()
        {
            var serviceCallLineItem = GetSaved<ServiceCallLineItem>(x => x.ServiceCallId = _serviceCall.ServiceCallId);
            var purchaseOrder = GetSaved<PurchaseOrder>(x => x.ServiceCallLineItemId = serviceCallLineItem.ServiceCallLineItemId);

            var command = new ServiceCallDenyCommand
            {
                ServiceCallId = _serviceCall.ServiceCallId
            };
            Send(command);
        }

        [Test, ExpectedException(typeof(DeleteServiceCallException))]
        public void ServiceCall_DenialWithPayment_ShouldFail()
        {
            var serviceCallLineItem = GetSaved<ServiceCallLineItem>(x => x.ServiceCallId = _serviceCall.ServiceCallId);
            var payment = GetSaved<Payment>(x => x.ServiceCallLineItemId = serviceCallLineItem.ServiceCallLineItemId);

            var command = new ServiceCallDenyCommand
            {
                ServiceCallId = _serviceCall.ServiceCallId
            };
            Send(command);
        }
    }
}