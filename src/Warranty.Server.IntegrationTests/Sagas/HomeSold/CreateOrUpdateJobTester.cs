using System;
using System.Linq;
using AutoMapper;
using Moq;
using NUnit.Framework;
using Should;
using TIPS.Commands.Requests;
using TIPS.Commands.Responses;
using Warranty.Core.Entities;
using Warranty.Core.Services;
using Warranty.Server.Handlers.Jobs;
using Warranty.Server.Sagas;

namespace Warranty.Server.IntegrationTests.Sagas.HomeSold
{
    [TestFixture]
    public class CreateOrUpdateJobTester : UseDummyBus
    {
        private const string Job_Exists = "2837287382";
        private const string Job_DoesNotExist = "8847263432";

        [TestFixtureSetUp]
        public void Setup()
        {
            JobService = new Mock<IJobService>();
            JobService.Setup(m => m.GetJobByNumber(Job_Exists))
                .Returns(new Job { JobNumber = Job_Exists})
                .Verifiable();

            JobService.Setup(m => m.GetJobByNumber(Job_DoesNotExist))
                .Returns(null as Job)
                .Verifiable();

            JobService.Setup(m => m.CreateJob(It.IsAny<Job>()))
                .Returns(new Job { JobNumber = Job_DoesNotExist })
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

            SagaData = new HomeSoldSagaData
            {
                JobSaleDetails = new JobSaleDetailsResponse(),
                Community = new Community()
            };

            Saga = new HomeSoldSaga(communityService.Object, JobService.Object, EmployeeService.Object, HomeOwnerService.Object, taskService.Object)
            {
                Data = SagaData,
                Bus = Bus
            };

            Mapper.Initialize(m => m.AddProfile(new MappingProfile()));
        }

        public Mock<IHomeOwnerService> HomeOwnerService { get; set; }

        public Mock<IEmployeeService> EmployeeService { get; set; }

        public Mock<IJobService> JobService { get; set; }

        public HomeSoldSagaData SagaData { get; set; }

        public HomeSoldSaga Saga { get; set; }

        [Test]
        public void ShouldSetPropertiesWhenJobFound()
        {
            SagaData.NewJob = null;
            JobService.ResetCalls();
            EmployeeService.ResetCalls();

            var message = new HomeSoldSaga_CreateOrUpdateJob(Job_Exists);
            Saga.Handle(message);
            SagaData.NewJob.ShouldNotBeNull();
        }

        [Test]
        public void ShouldUpdateExistingJobWhenFound()
        {
            SagaData.NewJob = null;
            SagaData.JobNumber = Job_Exists;
            JobService.ResetCalls();
            EmployeeService.ResetCalls();

            var message = new HomeSoldSaga_CreateOrUpdateJob(Job_Exists);
            Saga.Handle(message);
            
            EmployeeService.Verify(m => m.GetEmployeeByNumber(It.IsAny<int?>()), Times.Once);
            JobService.Verify(m => m.UpdateExistingJob(It.IsAny<Job>()), Times.Once);
            HomeOwnerService.Verify(m => m.RemoveHomeOwner(It.IsAny<Job>()), Times.Once);
        }

        [Test]
        public void ShouldCreateJobWhenNotFound()
        {
            SagaData.NewJob = null;
            SagaData.JobNumber = Job_DoesNotExist;
            JobService.ResetCalls();
            EmployeeService.ResetCalls();

            var message = new HomeSoldSaga_CreateOrUpdateJob(Job_DoesNotExist);
            Saga.Handle(message);

            EmployeeService.Verify(m => m.GetEmployeeByNumber(It.IsAny<int?>()), Times.Once);
            JobService.Verify(m => m.CreateJob(It.IsAny<Job>()), Times.Once);
            HomeOwnerService.Verify(m => m.RemoveHomeOwner(It.IsAny<Job>()), Times.Once);
        }

        [Test]
        public void ShouldSendToGetHomeAfterUpdatingJob()
        {
            SagaData.NewJob = null;
            SagaData.ContactId = Guid.NewGuid();
            SagaData.JobNumber = Job_Exists;
            JobService.ResetCalls();
            EmployeeService.ResetCalls();

            var message = new HomeSoldSaga_CreateOrUpdateJob(Job_DoesNotExist);
            Saga.Handle(message);

            var sentMessage = Bus.SentMessages.OfType<RequestHomeBuyerDetails>().FirstOrDefault(m => m.ContactId == SagaData.ContactId);
            sentMessage.ShouldNotBeNull();
        }

        [Test]
        public void ShouldSendToGetHomeAfterCreatingJob()
        {
            SagaData.NewJob = null;
            SagaData.ContactId = Guid.NewGuid();
            SagaData.JobNumber = Job_DoesNotExist;
            JobService.ResetCalls();
            EmployeeService.ResetCalls();

            var message = new HomeSoldSaga_CreateOrUpdateJob(Job_DoesNotExist);
            Saga.Handle(message);

            var sentMessage = Bus.SentMessages.OfType<RequestHomeBuyerDetails>().FirstOrDefault(m => m.ContactId == SagaData.ContactId);
            sentMessage.ShouldNotBeNull();
        }
    }
}