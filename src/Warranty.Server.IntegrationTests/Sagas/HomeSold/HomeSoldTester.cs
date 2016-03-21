using System;
using System.Linq;
using log4net;
using Moq;
using NUnit.Framework;
using Should;
using TIPS.Commands.Requests;
using Warranty.Core.Services;
using Warranty.Server.Handlers.Jobs;
using Warranty.Server.Sagas;

namespace Warranty.Server.IntegrationTests.Sagas.HomeSold
{
    [TestFixture]
    public class HomeSoldTester : UseDummyBus
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            var jobService = new Mock<IJobService>();
            var homeOwnerService = new Mock<IHomeOwnerService>();
            var taskService = new Mock<ITaskService>();
            var employeeService = new Mock<IEmployeeService>();
            var communityService = new Mock<ICommunityService>();
            var log = new Mock<ILog>();

            SagaData = new HomeSoldSagaData();
            Saga = new HomeSoldSaga(communityService.Object, jobService.Object, employeeService.Object,homeOwnerService.Object, taskService.Object, log.Object)
            {
                Data = SagaData,
                Bus = Bus
            };
        }

        public HomeSoldSagaData SagaData { get; set; }

        public HomeSoldSaga Saga { get; set; }

        [Test]
        public void ShouldSetDataFields()
        {
            SagaData.ContactId = Guid.Empty;
            SagaData.JobNumber = null;
            var message = new TIPS.Events.JobEvents.HomeSold()
            {
                ContactId = Guid.NewGuid(),
                JobNumber = "328728372873"
            };

            Saga.Handle(message);
            SagaData.ContactId.ShouldNotEqual(Guid.Empty);
            SagaData.JobNumber.ShouldNotBeNull();
        }

        [Test]
        public void ShouldSendToGetJobDetails()
        {
            SagaData.ContactId = Guid.Empty;
            SagaData.JobNumber = null;
            var message = new TIPS.Events.JobEvents.HomeSold()
            {
                ContactId = Guid.NewGuid(),
                JobNumber = "56748392",
                SaleId = 8373827327
            };
            Saga.Handle(message);

            var sentMessage = Bus.SentMessages.OfType<RequestJobSaleDetails>().FirstOrDefault(m => m.SaleId == message.SaleId);
            sentMessage.ShouldNotBeNull();
        }
    }
}