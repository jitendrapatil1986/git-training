using System;
using System.Linq;
using AutoMapper;
using Fake.Bus;
using log4net;
using Moq;
using NUnit.Framework;
using Should;
using TIPS.Commands.Requests;
using TIPS.Commands.Responses;
using Warranty.Core;
using Warranty.Core.Entities;
using Warranty.Core.Services;
using Warranty.Server.Handlers.Jobs;
using Warranty.Server.Sagas;

namespace Warranty.Server.IntegrationTests.Sagas.HomeSold
{
    [TestFixture]
    public class HomeSoldSaga_CreateOrUpdateJobTester
    {
        private const string Job_Exists = "2837287382";
        private const string Job_DoesNotExist = "8847263432";

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            Mapper.Initialize(m => m.AddProfile(new MappingProfile()));
        }

        [SetUp]
        public void Setup()
        {
            JobService = new Mock<IJobService>();
            JobService.Setup(m => m.GetJobByNumber(Job_Exists))
                .Returns(new Job
                {
                    JobId = Guid.NewGuid(),
                    JobNumber = Job_Exists
                })
                .Verifiable();

            JobService.Setup(m => m.GetJobByNumber(Job_DoesNotExist))
                .Returns(null as Job)
                .Verifiable();

            JobService.Setup(m => m.CreateJob(It.IsAny<Job>()))
                .Returns(new Job
                {
                    JobId = Guid.NewGuid(),
                    JobNumber = Job_DoesNotExist
                })
                .Verifiable();

            JobService.Setup(m => m.UpdateExistingJob(It.IsAny<Job>()))
                .Verifiable();

            EmployeeService = new Mock<IEmployeeService>();
            EmployeeService.Setup(m => m.GetEmployeeByNumber(It.IsAny<int?>()))
                .Returns(new Employee())
                .Verifiable();

            HomeOwnerService = new Mock<IHomeOwnerService>();
            HomeOwnerService.Setup(m => m.RemoveHomeOwner(It.IsAny<Job>()))
                .Verifiable();

            var taskService = new Mock<ITaskService>();
            var communityService = new Mock<ICommunityService>();
            var log = new Mock<ILog>();
            var mediator = new Mock<IMediator>();

            Bus = new FakeBus();

            SagaData = new HomeSoldSagaData
            {
                JobSaleDetails = new JobSaleDetailsResponse(),
                CommunityReferenceId = Guid.NewGuid()
            };

            Saga = new HomeSoldSaga(communityService.Object, JobService.Object, EmployeeService.Object, HomeOwnerService.Object, taskService.Object, log.Object, mediator.Object)
            {
                Data = SagaData,
                Bus = Bus
            };
        }

        public FakeBus Bus { get; set; }

        public Mock<IHomeOwnerService> HomeOwnerService { get; set; }

        public Mock<IEmployeeService> EmployeeService { get; set; }

        public Mock<IJobService> JobService { get; set; }

        public HomeSoldSagaData SagaData { get; set; }

        public HomeSoldSaga Saga { get; set; }

        [Test]
        public void ShouldSetPropertiesWhenJobFound()
        {
            SagaData.JobReferenceId = Guid.Empty;
            SagaData.JobNumber = Job_Exists;

            JobService.ResetCalls();
            EmployeeService.ResetCalls();

            var message = new HomeSoldSaga_CreateOrUpdateJob(8723837273287);
            Saga.Handle(message);
            SagaData.JobReferenceId.ShouldNotEqual(Guid.Empty);
        }

        [Test]
        public void ShouldUpdateExistingJobWhenFound()
        {
            SagaData.JobReferenceId = Guid.Empty;
            SagaData.JobNumber = Job_Exists;
            JobService.ResetCalls();
            EmployeeService.ResetCalls();
            HomeOwnerService.ResetCalls();

            var message = new HomeSoldSaga_CreateOrUpdateJob(8723837273287);
            Saga.Handle(message);
            
            EmployeeService.Verify(m => m.GetEmployeeByNumber(It.IsAny<int?>()), Times.Once);
            JobService.Verify(m => m.UpdateExistingJob(It.IsAny<Job>()), Times.Once);
            HomeOwnerService.Verify(m => m.RemoveHomeOwner(It.IsAny<Job>()), Times.Once);
        }

        [Test]
        public void ShouldCreateJobWhenNotFound()
        {
            SagaData.JobReferenceId = Guid.Empty;
            SagaData.JobNumber = Job_DoesNotExist;
            JobService.ResetCalls();
            EmployeeService.ResetCalls();

            var message = new HomeSoldSaga_CreateOrUpdateJob(12837812738273);
            Saga.Handle(message);

            EmployeeService.Verify(m => m.GetEmployeeByNumber(It.IsAny<int?>()), Times.Once);
            JobService.Verify(m => m.CreateJob(It.IsAny<Job>()), Times.Once);
            HomeOwnerService.Verify(m => m.RemoveHomeOwner(It.IsAny<Job>()), Times.Once);
        }

        [Test]
        public void ShouldSendToGetHomeAfterUpdatingJob()
        {
            SagaData.JobReferenceId = Guid.Empty;
            SagaData.ContactId = Guid.NewGuid();
            SagaData.JobNumber = Job_Exists;
            JobService.ResetCalls();
            EmployeeService.ResetCalls();

            var message = new HomeSoldSaga_CreateOrUpdateJob(127831827387123);
            Saga.Handle(message);

            var sentMessage = Bus.SentMessages<RequestHomeBuyerDetails>().FirstOrDefault(m => m.ContactId == SagaData.ContactId);
            sentMessage.ShouldNotBeNull();
        }

        [Test]
        public void ShouldSendToGetHomeAfterCreatingJob()
        {
            SagaData.JobReferenceId = Guid.Empty;
            SagaData.ContactId = Guid.NewGuid();
            SagaData.JobNumber = Job_DoesNotExist;
            JobService.ResetCalls();
            EmployeeService.ResetCalls();

            var message = new HomeSoldSaga_CreateOrUpdateJob(123872387237);
            Saga.Handle(message);

            var sentMessage = Bus.SentMessages<RequestHomeBuyerDetails>().FirstOrDefault(m => m.ContactId == SagaData.ContactId);
            sentMessage.ShouldNotBeNull();
        }
    }
}