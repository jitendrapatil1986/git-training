using System;
using System.Collections.Generic;
using AutoMapper;
using Fake.Bus;
using Moq;
using NUnit.Framework;
using Should;
using StructureMap;
using TIPS.Commands.Responses;
using TIPS.Events.Models;
using Warranty.Core;
using Warranty.Core.Entities;
using Warranty.Core.Features.Homeowner;
using Warranty.Core.Services;
using Warranty.Server.Handlers.Jobs;
using Warranty.Server.Sagas;
using Job = Warranty.Core.Entities.Job;

namespace Warranty.Server.IntegrationTests.Sagas.BuyerTransferredToNewLot
{
    [TestFixture]
    public class BuyerTransferredToNewLotSaga_HomeBuyerDetailsResponseTester
    {
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
        public void FixtureSetup()
        {
            Mapper.Initialize(m => m.AddProfile(new MappingProfile()));
        }

        [SetUp]
        public void Setup()
        {
            Mediator = new Mock<IMediator>();
            var employeeService = new Mock<IEmployeeService>();
            var communityService = new Mock<ICommunityService>();
            var log = new Mock<log4net.ILog>();

            Job_Reference = Guid.NewGuid();
            HomeOwner_Exists = Guid.NewGuid();
            HomeOwner_DoesNotExist = Guid.NewGuid();

            JobService = new Mock<IJobService>();
            JobService.Setup(m => m.GetJobById(Job_Reference))
                .Returns(new Job { JobId = Job_Reference })
                .Verifiable();

            HomeOwnerService = new Mock<IHomeOwnerService>();

            Mediator.Setup(m => m.Send(It.IsAny<CreateNewHomeOwnerCommand>()))
                .Returns(new HomeOwner {HomeOwnerId = Guid.NewGuid()});

            HomeOwnerService.Setup(a => a.GetByHomeOwnerId(HomeOwner_DoesNotExist))
                .Returns(null as HomeOwner);

            HomeOwnerService.Setup(a => a.GetByHomeOwnerId(HomeOwner_Exists))
                .Returns(new HomeOwner {HomeOwnerId = HomeOwner_Exists});

            HomeOwnerService.Setup(a => a.AssignToJob(It.IsAny<HomeOwner>(), It.IsAny<Job>()));

            TaskService = new Mock<ITaskService>();
            TaskService.Setup(m => m.CreateTasks(It.IsAny<Guid>()));
            Bus = new FakeBus();

            SagaData = new BuyerTransferredToNewLotSagaData();

            Saga = new BuyerTransferredToNewLotSaga(JobService.Object, HomeOwnerService.Object, TaskService.Object, employeeService.Object, communityService.Object, log.Object, Mediator.Object)
            {
                Bus = Bus,
                Data = SagaData
            };
        }

        public Mock<IMediator> Mediator { get; set; }

        public Guid HomeOwner_DoesNotExist { get; set; }
        public Guid HomeOwner_Exists { get; set; }
        public Guid Job_Reference { get; set; }

        public FakeBus Bus { get; set; }
        
        [Test]
        public void ShouldCreateNewHomeOwner()
        {
            SagaData.JobIdReference = Job_Reference;
            Saga.Handle(Response);

            JobService.Verify(a => a.GetJobById(Job_Reference), Times.Once);
            Mediator.Verify(a => a.Send(It.IsAny<CreateNewHomeOwnerCommand>()), Times.Once);
        }

        [Test]
        public void ShouldAssignNewHomeOwnerToJob()
        {
            SagaData.JobIdReference = Job_Reference;
            Saga.Handle(Response);

            HomeOwnerService.Verify(a => a.AssignToJob(It.IsAny<HomeOwner>(), It.IsAny<Job>()), Times.Once);
        }

        [Test]
        public void ShouldAssignExistingOwnerToJob()
        {
            SagaData.JobIdReference = Job_Reference;
            Saga.Handle(Response);

            HomeOwnerService.Verify(a => a.AssignToJob(It.IsAny<HomeOwner>(), It.IsAny<Job>()), Times.Once);
        }

        [Test]
        public void ShouldCreateTasksOnJobForNewHomwOwner()
        {
            SagaData.JobIdReference = Job_Reference;
            Saga.Handle(Response);

            TaskService.Verify(a => a.CreateTasks(Job_Reference), Times.Once);
        }

        [Test]
        public void ShouldCreateTasksOnJobForExistingHomeOwner()
        {
            SagaData.JobIdReference = Job_Reference;
            Saga.Handle(Response);

            TaskService.Verify(a => a.CreateTasks(Job_Reference), Times.Once);
        }

        [Test]
        public void ShouldMarkSagaCompleteForNewHomeOwner()
        {
            SagaData.JobIdReference = Job_Reference;
            Saga.Handle(Response);

            Saga.Completed.ShouldEqual(true);
        }

        [Test]
        public void ShouldMarkSagaCompleteForExistingHomeOwner()
        {
            SagaData.JobIdReference = Job_Reference;
            Saga.Handle(Response);

            Saga.Completed.ShouldEqual(true);

        }
        
    }
}