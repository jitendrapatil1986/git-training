using System;
using AutoMapper;
using Moq;
using NUnit.Framework;
using Should;
using TIPS.Commands.Responses;
using Warranty.Core.Entities;
using Warranty.Core.Services;
using Warranty.Server.Handlers.Jobs;
using Warranty.Server.Sagas;

namespace Warranty.Server.IntegrationTests.Sagas.BuyerTransferredToNewLot
{
    [TestFixture]
    public class JobSaleDetailsResponseTester : UseDummyBus
    {
        private readonly int? Builder_Exists = 827838273;
        private const string Community_Exists = "278362763263";
        private readonly Job ExistingJob = new Job
        {
            JobId = Guid.NewGuid(),
            JobNumber = "72632763",
            CreatedBy = "Someone",
            CreatedDate = DateTime.UtcNow.AddDays(-2)
        };
        
        public BuyerTransferredToNewLotSagaData SagaData { get; set; }
        public BuyerTransferredToNewLotSaga Saga { get; set; }

        [TestFixtureSetUp]
        public void Setup()
        {
            Mapper.Initialize(m => m.AddProfile(new MappingProfile()));

            JobService = new Mock<IJobService>();
            JobService.Setup(m => m.GetJobByNumber(ExistingJob.JobNumber)).Returns(ExistingJob).Verifiable();
            JobService.Setup(m => m.CreateJob(It.IsAny<Job>())).Returns(new Job()).Verifiable();
            JobService.Setup(m => m.Save(It.IsAny<Job>())).Verifiable();

            var homeOwnerService = new Mock<IHomeOwnerService>();
            var taskService = new Mock<ITaskService>();

            EmployeeService = new Mock<IEmployeeService>();
            EmployeeService.Setup(m => m.GetEmployeeByNumber(Builder_Exists))
                .Returns(new Employee
                {
                    EmployeeId = Guid.NewGuid()
                })
                .Verifiable();

            CommunityService = new Mock<ICommunityService>();
            CommunityService.Setup(m => m.GetCommunityByNumber(Community_Exists))
                .Returns(new Community()
                {
                    CommunityId = Guid.NewGuid()
                })
                .Verifiable();

            var log = new Mock<log4net.ILog>();

            SagaData = new BuyerTransferredToNewLotSagaData();

            Saga = new BuyerTransferredToNewLotSaga(JobService.Object, homeOwnerService.Object, taskService.Object, EmployeeService.Object, CommunityService.Object, log.Object)
            {
                Bus = Bus,
                Data = SagaData
            };
        }

        public Mock<IJobService> JobService { get; set; }

        public Mock<ICommunityService> CommunityService { get; set; }

        public Mock<IEmployeeService> EmployeeService { get; set; }

        [Test]
        public void ShouldSetSagaDataFromResponse()
        {
            SagaData.NewJob = null;

            var message = new JobSaleDetailsResponse();
            Saga.Handle(message);

            SagaData.NewJob.ShouldNotBeNull();
        }

        [Test]
        public void ShouldSetBuilderIdWhenFound()
        {
            SagaData.NewJob = null;

            var message = new JobSaleDetailsResponse
            {
                BuilderEmployeeID = Builder_Exists
            };

            Saga.Handle(message);
            SagaData.NewJob.ShouldNotBeNull();
            
            EmployeeService.Verify(e => e.GetEmployeeByNumber(Builder_Exists), Times.Once);
        }

        [Test]
        public void ShouldSetCommunityIdWhenFound()
        {
            SagaData.NewJob = null;

            var message = new JobSaleDetailsResponse
            {
                CommunityNumber = Community_Exists
            };

            Saga.Handle(message);
            SagaData.NewJob.ShouldNotBeNull();

            CommunityService.Verify(e => e.GetCommunityByNumber(Community_Exists), Times.Once);
        }

        [Test]
        public void ShouldUpdateExistingJobWhenFound()
        {
            SagaData.NewJob = null;
            JobService.ResetCalls();

            var message = new JobSaleDetailsResponse
            {
                JobNumber = ExistingJob.JobNumber,
                AddressCity = "Houston",
                AddressLine1 = "123 Main St",
                AddressStateAbbreviation = "TX",
                AddressZipCode = "77571"
            };

            Saga.Handle(message);
            SagaData.NewJob.ShouldNotBeNull();

            JobService.Verify(m => m.GetJobByNumber(message.JobNumber), Times.Once);

            var newJob = SagaData.NewJob;
            newJob.ShouldNotBeNull();
            newJob.JobId.ShouldEqual(ExistingJob.JobId);
            newJob.CreatedBy.ShouldEqual(ExistingJob.CreatedBy);
            newJob.CreatedDate.ShouldEqual(ExistingJob.CreatedDate);
            newJob.JobNumber.ShouldEqual(ExistingJob.JobNumber);

            newJob.AddressLine.ShouldEqual(message.AddressLine1);
            newJob.City.ShouldEqual(message.AddressCity);
            newJob.StateCode.ShouldEqual(message.AddressStateAbbreviation);
            newJob.PostalCode.ShouldEqual(message.AddressZipCode);

            JobService.Verify(m => m.Save(It.IsAny<Job>()), Times.Once);
            JobService.Verify(m => m.CreateJob(It.IsAny<Job>()), Times.Never);
        }

        [Test]
        public void ShouldCreateNewJobWhenNotFound()
        {
            SagaData.NewJob = null;
            JobService.ResetCalls();

            var message = new JobSaleDetailsResponse
            {
                JobNumber = "88273758"
            };

            Saga.Handle(message);
            SagaData.NewJob.ShouldNotBeNull();

            JobService.Verify(m => m.GetJobByNumber(message.JobNumber), Times.Once);

            var newJob = SagaData.NewJob;
            newJob.ShouldNotBeNull();
            
            JobService.Verify(m => m.Save(It.IsAny<Job>()), Times.Never);
            JobService.Verify(m => m.CreateJob(It.IsAny<Job>()), Times.Once);
        }
    }
}