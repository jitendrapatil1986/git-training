using System;
using System.Collections.Generic;
using AutoMapper;
using Moq;
using NUnit.Framework;
using Should;
using TIPS.Commands.Responses;
using TIPS.Events.Models;
using Warranty.Core.Entities;
using Warranty.Core.Services;
using Warranty.Server.Handlers.Jobs;
using Warranty.Server.Sagas;
using Job = Warranty.Core.Entities.Job;

namespace Warranty.Server.IntegrationTests.Sagas.BuyerTransferredToNewLot
{
    [TestFixture]
    public class HomeBuyerDetailsResponseTester : UseDummyBus
    {
        private readonly Guid Job_Reference = Guid.NewGuid();
        private readonly Guid HomeOwner_DoesNotExist = Guid.NewGuid();
        private readonly Guid HomeOwner_Exists = Guid.NewGuid();
        public BuyerTransferredToNewLotSagaData SagaData { get; set; }
        public BuyerTransferredToNewLotSaga Saga { get; set; }
        public Mock<IHomeOwnerService> HomeOwnerService { get; set; }
        public Mock<IJobService> JobService { get; set; }
        public Mock<ITaskService> TaskService { get; set; }

        private static HomeBuyerDetailsResponse Response
        {
            get
            {
                return new HomeBuyerDetailsResponse
                {
                    FirstName = "John",
                    LastName = "Smith",
                    PhoneNumbers = new List<PhoneNumber>
                    {
                        new PhoneNumber { IsPrimary = false, Number = "555-555-5555", Type = "HOME"},
                        new PhoneNumber { IsPrimary = true, Number = "111-111-1111", Type = "WORK"}
                    },
                    EmailAddresses = new List<Email>
                    {
                        new Email { Address = "notprimary@email.com", IsPrimary = false },
                        new Email { Address = "primary@email.com", IsPrimary = true }
                    }
                };
            }
        }

        [TestFixtureSetUp]
        public void Setup()
        {
            Mapper.Initialize(m => m.AddProfile(new MappingProfile()));

            JobService = new Mock<IJobService>();
            JobService.Setup(m => m.GetJobById(Job_Reference))
                .Returns(new Job { JobId = Job_Reference })
                .Verifiable();

            HomeOwnerService = new Mock<IHomeOwnerService>();
            HomeOwnerService.Setup(p => p.Create(It.IsAny<HomeOwner>()))
               .Returns(new HomeOwner { HomeOwnerId = Guid.NewGuid() })
               .Verifiable();
            HomeOwnerService.Setup(a => a.GetByHomeOwnerId(HomeOwner_DoesNotExist))
                .Returns(null as HomeOwner)
                .Verifiable();
            HomeOwnerService.Setup(a => a.GetByHomeOwnerId(HomeOwner_Exists))
                .Returns(new HomeOwner { HomeOwnerId = HomeOwner_Exists})
                .Verifiable();
            HomeOwnerService.Setup(a => a.AssignToJob(It.IsAny<HomeOwner>(), It.IsAny<Job>()))
                .Verifiable();

            TaskService = new Mock<ITaskService>();
            TaskService.Setup(m => m.CreateTasks(It.IsAny<Guid>()))
                .Verifiable();

            var employeeService = new Mock<IEmployeeService>();
            var communityService = new Mock<ICommunityService>();
            var log = new Mock<log4net.ILog>();

            SagaData = new BuyerTransferredToNewLotSagaData();

            Saga = new BuyerTransferredToNewLotSaga(JobService.Object, HomeOwnerService.Object, TaskService.Object, employeeService.Object, communityService.Object, log.Object)
            {
                Bus = Bus,
                Data = SagaData
            };
        }
        private void ResetCalls()
        {
            JobService.ResetCalls();
            TaskService.ResetCalls();
            HomeOwnerService.ResetCalls();
        }

        [Test]
        public void ShouldCreateNewHomeOwner()
        {
            ResetCalls();
            SagaData.JobIdReference = Job_Reference;
            SagaData.HomeOwnerReference = HomeOwner_DoesNotExist;

            Saga.Handle(Response);

            JobService.Verify(a => a.GetJobById(Job_Reference), Times.Once);
            HomeOwnerService.Verify(a => a.GetByHomeOwnerId(HomeOwner_DoesNotExist), Times.Once);
            HomeOwnerService.Verify(a => a.Create(It.IsAny<HomeOwner>()), Times.Once);
        }

        [Test]
        public void ShouldUpdateExistingHomeOwner()
        {
            ResetCalls();
            SagaData.JobIdReference = Job_Reference;
            SagaData.HomeOwnerReference = HomeOwner_Exists;

            Saga.Handle(Response);

            JobService.Verify(a => a.GetJobById(Job_Reference), Times.Once);
            HomeOwnerService.Verify(a => a.GetByHomeOwnerId(HomeOwner_Exists), Times.Once);
            HomeOwnerService.Verify(a => a.Create(It.IsAny<HomeOwner>()), Times.Never);
        }

        [Test]
        public void ShouldAssignNewHomeOwnerToJob()
        {
            ResetCalls();
            SagaData.JobIdReference = Job_Reference;
            SagaData.HomeOwnerReference = HomeOwner_DoesNotExist;

            Saga.Handle(Response);

            HomeOwnerService.Verify(a => a.AssignToJob(It.IsAny<HomeOwner>(), It.IsAny<Job>()), Times.Once);
        }

        [Test]
        public void ShouldAssignExistingOwnerToJob()
        {
            ResetCalls();
            SagaData.JobIdReference = Job_Reference;
            SagaData.HomeOwnerReference = HomeOwner_Exists;

            Saga.Handle(Response);

            HomeOwnerService.Verify(a => a.AssignToJob(It.IsAny<HomeOwner>(), It.IsAny<Job>()), Times.Once);
        }

        [Test]
        public void ShouldCreateTasksOnJobForNewHomwOwner()
        {
            ResetCalls();
            SagaData.JobIdReference = Job_Reference;
            SagaData.HomeOwnerReference = HomeOwner_DoesNotExist;

            Saga.Handle(Response);

            TaskService.Verify(a => a.CreateTasks(Job_Reference), Times.Once);
        }

        [Test]
        public void ShouldCreateTasksOnJobForExistingHomeOwner()
        {
            ResetCalls();
            SagaData.JobIdReference = Job_Reference;
            SagaData.HomeOwnerReference = HomeOwner_Exists;

            Saga.Handle(Response);

            TaskService.Verify(a => a.CreateTasks(Job_Reference), Times.Once);
        }

        [Test]
        public void ShouldMarkSagaCompleteForNewHomeOwner()
        {
            ResetCalls();
            SagaData.JobIdReference = Job_Reference;
            SagaData.HomeOwnerReference = HomeOwner_DoesNotExist;

            Saga.Handle(Response);

            Saga.Completed.ShouldEqual(true);
        }

        [Test]
        public void ShouldMarkSagaCompleteForExistingHomeOwner()
        {
            ResetCalls();
            SagaData.JobIdReference = Job_Reference;
            SagaData.HomeOwnerReference = HomeOwner_Exists;

            Saga.Handle(Response);

            Saga.Completed.ShouldEqual(true);

        }
        
    }
}