using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Should;
using TIPS.Commands.Requests;
using Warranty.Core.Entities;
using Warranty.Core.Services;
using Warranty.Server.Handlers.Jobs;
using Warranty.Server.Sagas;

namespace Warranty.Server.IntegrationTests.Sagas.BuyerTransferredToNewLot
{
    [TestFixture]
    public class BuyerTransferredToNewLotTester : UseDummyBus
    {
        public BuyerTransferredToNewLotSagaData SagaData { get; set; }
        public BuyerTransferredToNewLotSaga Saga { get; set; }
        public Mock<IHomeOwnerService> HomeOwnerService { get; set; }

        private const string JobNumber_For_Unknown_HomeOwner = "123456789";
        private const string JobNumber_For_Known_HomeOwner = "99988877";

        [TestFixtureSetUp]
        public void Setup()
        {
            var jobService = new Mock<IJobService>();

            HomeOwnerService = new Mock<IHomeOwnerService>();
            HomeOwnerService.Setup(p => p.GetHomeOwnerByJobNumber(JobNumber_For_Unknown_HomeOwner))
                .Returns(null as HomeOwner)
                .Verifiable();

            HomeOwnerService.Setup(p => p.GetHomeOwnerByJobNumber(JobNumber_For_Known_HomeOwner))
               .Returns(KnownHomeOwner)
               .Verifiable();

            var taskService = new Mock<ITaskService>();
            var employeeService = new Mock<IEmployeeService>();
            var communityService = new Mock<ICommunityService>();
            var log = new Mock<log4net.ILog>();

            SagaData = new BuyerTransferredToNewLotSagaData();

            Saga = new BuyerTransferredToNewLotSaga(jobService.Object, HomeOwnerService.Object,taskService.Object, employeeService.Object, communityService.Object, log.Object)
            {
                Bus = Bus,
                Data = SagaData
            };
        }

        public HomeOwner KnownHomeOwner
        {
            get
            {
                return new HomeOwner
                {
                    HomeOwnerId = Guid.NewGuid(),
                    HomeOwnerName = "John Smith"
                };
            }
        }

        [Test]
        public void ShouldSetDataFields()
        {
            var message = new TIPS.Events.JobEvents.BuyerTransferredToNewLot
            {
                ContactId = Guid.NewGuid(),
                NewJobNumber = JobNumber_For_Unknown_HomeOwner,
                PreviousJobNumber = "Does Not Matter"
            };

            Saga.Handle(message);

            SagaData.NewJobNumber.ShouldEqual(message.NewJobNumber);
            SagaData.ContactId.ShouldEqual(message.ContactId);
            SagaData.PreviousJobNumber.ShouldEqual(message.PreviousJobNumber);
        }

        [Test]
        public void ShouldSendTipsRequestWhenHomeOwnerNotFound()
        {
            var message = new TIPS.Events.JobEvents.BuyerTransferredToNewLot
            {
                ContactId = Guid.NewGuid(),
                NewJobNumber = JobNumber_For_Unknown_HomeOwner,
                PreviousJobNumber = "Does Not Matter",
                SaleId = 23132738273827
            };

            Saga.Handle(message);

            var sentMessage = Bus.SentMessages.OfType<RequestJobSaleDetails>().FirstOrDefault(m => m.SaleId  == message.SaleId);
            sentMessage.ShouldNotBeNull();
        }

        [Test]
        public void ShouldSetHomeOwnerDataWhenFound()
        {
            SagaData.HomeOwnerReference = Guid.Empty;

            var message = new TIPS.Events.JobEvents.BuyerTransferredToNewLot
            {
                ContactId = Guid.NewGuid(),
                NewJobNumber = "Does Not Matter",
                PreviousJobNumber = JobNumber_For_Known_HomeOwner
            };

            Saga.Handle(message);

            SagaData.HomeOwnerReference.ShouldNotEqual(Guid.Empty);
        }

        [Test]
        public void ShouldSendToRemoveExistingHomeOwnerWhenFound()
        {
            var message = new TIPS.Events.JobEvents.BuyerTransferredToNewLot
            {
                ContactId = Guid.NewGuid(),
                NewJobNumber = "Does Not Matter",
                PreviousJobNumber = JobNumber_For_Known_HomeOwner,
                SaleId = 75645638
            };

            Saga.Handle(message);

            var sentMessage = Bus.SentLocalMessages.OfType<BuyerTransferredToNewLotSaga_RemoveExistingHomeOwner>()
                .FirstOrDefault(m => m.SaleId == message.SaleId);
            sentMessage.ShouldNotBeNull();
        }
        
    }
}