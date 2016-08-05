using System;
using System.Linq;
using Fake.Bus;
using Moq;
using NUnit.Framework;
using Should;
using TIPS.Commands.Requests;
using Warranty.Core;
using Warranty.Core.Entities;
using Warranty.Core.Services;
using Warranty.Server.Handlers.Jobs;
using Warranty.Server.Sagas;

namespace Warranty.Server.IntegrationTests.Sagas.BuyerTransferredToNewLot
{
    [TestFixture]
    public class BuyerTransferredToNewLotSaga_BuyerTransferredTonewLotTester
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
            
            Mediator = new Mock<IMediator>();

            Bus = new FakeBus();

            SagaData = new BuyerTransferredToNewLotSagaData();

            Saga = new BuyerTransferredToNewLotSaga(jobService.Object, HomeOwnerService.Object,taskService.Object, employeeService.Object, communityService.Object, log.Object, Mediator.Object)
            {
                Bus = Bus,
                Data = SagaData
            };
        }

        public Mock<IMediator> Mediator { get; set; }

        public FakeBus Bus { get; set; }

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

            var sentMessage = Bus.SentMessages<RequestJobSaleDetails>().FirstOrDefault(m => m.SaleId  == message.SaleId);
            sentMessage.ShouldNotBeNull();
        }
    }
}