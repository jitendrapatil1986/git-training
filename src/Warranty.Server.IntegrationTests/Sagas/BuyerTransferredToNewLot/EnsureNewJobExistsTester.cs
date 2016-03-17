using System.Linq;
using Moq;
using NServiceBus.Saga;
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
    public class EnsureNewJobExistsTester : UseDummyBus
    {
        private const string JobNumber_DoesNotExist = "7463527";
        private const string JobNumber_Exists = "88473562";
        public BuyerTransferredToNewLotSaga Saga { get; set; }
        public BuyerTransferredToNewLotSagaData SagaData { get; set; }

        [TestFixtureSetUp]
        public void Setup()
        {
            var jobService = new Mock<IJobService>();
            jobService.Setup(m => m.GetJobByNumber(JobNumber_DoesNotExist))
                .Returns(null as Job)
                .Verifiable();

            jobService.Setup(m => m.GetJobByNumber(JobNumber_Exists))
                .Returns(new Job())
                .Verifiable();

            var homeOwnerService = new Mock<IHomeOwnerService>();
            var taskService = new Mock<ITaskService>();
            var employeeService = new Mock<IEmployeeService>();
            var communityService = new Mock<ICommunityService>();

            SagaData = new BuyerTransferredToNewLotSagaData();

            Saga = new BuyerTransferredToNewLotSaga(jobService.Object, homeOwnerService.Object, taskService.Object, employeeService.Object, communityService.Object)
            {
                Bus = Bus,
                Data = SagaData
            };
        }

        [Test]
        public void ShouldRequestJobFromTipsWhenNotFound()
        {
            SagaData.NewJobNumber = JobNumber_DoesNotExist;
            var message = new BuyerTransferredToNewLotSaga_EnsureNewJobExists(JobNumber_DoesNotExist);

            Saga.Handle(message);

            var sentMessage = Bus.SentMessages.OfType<RequestJobSaleDetails>().FirstOrDefault(m => m.JobNumber == JobNumber_DoesNotExist);
            sentMessage.ShouldNotBeNull();
        }

        [Test]
        public void ShouldSetSagaDataWhenFound()
        {
            SagaData.NewJob = null;
            SagaData.NewJobNumber = JobNumber_Exists;
            var message = new BuyerTransferredToNewLotSaga_EnsureNewJobExists(JobNumber_Exists);

            Saga.Handle(message);

            SagaData.NewJob.ShouldNotBeNull();
        }

        [Test]
        public void ShouldSendToAssignTasksWhenFound()
        {
            SagaData.NewJobNumber = JobNumber_Exists;
            var message = new BuyerTransferredToNewLotSaga_EnsureNewJobExists(JobNumber_Exists);

            Saga.Handle(message);

            var sentMessage = Bus.SentLocalMessages.OfType<BuyerTransferredToNewLotSaga_AssignHomeownerAndTasks>().FirstOrDefault(m => m.NewJobNumber == JobNumber_Exists);
            sentMessage.ShouldNotBeNull();
        }
        
    }
}