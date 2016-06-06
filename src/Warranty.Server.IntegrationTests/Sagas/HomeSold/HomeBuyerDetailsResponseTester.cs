using System;
using AutoMapper;
using log4net;
using Moq;
using NUnit.Framework;
using Should;
using TIPS.Commands.Responses;
using Warranty.Core.Entities;
using Warranty.Core.Services;
using Warranty.Server.Sagas;
using Job = Warranty.Core.Entities.Job;

namespace Warranty.Server.IntegrationTests.Sagas.HomeSold
{
    [TestFixture]
    public class HomeBuyerDetailsResponseTester : HandlerTester<HomeBuyerDetailsResponse>
    {
        private HomeSoldSaga Saga { get; set; }
        private readonly Guid _homeOwnerExistsForJobId = Guid.NewGuid();

        [TestFixtureSetUp]
        public void Setup()
        {
            Mapper.Initialize(m => m.AddProfile(new MappingProfile()));

            var taskService = new Mock<ITaskService>();
            var employeeService = new Mock<IEmployeeService>();
            var communityService = new Mock<ICommunityService>();

            var homeOwnerService = new HomeOwnerService(TestDatabase);
            var jobService = new JobService(TestDatabase, employeeService.Object, communityService.Object);

            var log = new Mock<ILog>();
            log.Setup(m => m.ErrorFormat(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<object>())).Verifiable();

            Saga = new HomeSoldSaga(communityService.Object, jobService, employeeService.Object, homeOwnerService, taskService.Object, log.Object)
            {
                Data = new HomeSoldSagaData(),
                Bus = new DummyBus(),
            };
        }

        [Test]
        public void ShouldUpdateHomeOwnerRecordIfItAlreadyExists()
        {

            var homeOwner = GetSaved<HomeOwner>(owner =>
            {
                owner.HomeOwnerNumber = 112233;
                owner.HomeOwnerName = "HomeOwner One";
            }); 

            var jobWithHomeOwner = GetSaved<Job>(job =>
            {
                job.JobId = _homeOwnerExistsForJobId;
                job.CurrentHomeOwnerId = homeOwner.HomeOwnerId;
                job.JobNumber = "001122";
            });

            homeOwner.JobId = jobWithHomeOwner.JobId;
            TestDatabase.Update(homeOwner);

            Saga.Data.JobReferenceId = jobWithHomeOwner.JobId;
            Saga.Handle(new HomeBuyerDetailsResponse
            {
                FirstName = "NewHome",
                LastName = "Owner",
            });

            var homeOwnerAfterResponseProcessed = TestDatabase.Single<HomeOwner>("WHERE HomeOwnerID = @0;", homeOwner.HomeOwnerId);

            homeOwnerAfterResponseProcessed.HomeOwnerNumber.ShouldEqual(homeOwner.HomeOwnerNumber);
            homeOwnerAfterResponseProcessed.HomeOwnerName.ShouldEqual(homeOwner.HomeOwnerName);
            homeOwnerAfterResponseProcessed.UpdatedDate.ShouldBeGreaterThan(homeOwner.UpdatedDate);
            homeOwnerAfterResponseProcessed.UpdatedBy.ShouldEqual(Constants.ENDPOINT_NAME);
        }

        [Test]
        public void ShouldInsertHomeOwnerRecordIfItDoesntExist()
        {
            var jobId = Guid.NewGuid();

            var jobWithNoHomeOwner = GetSaved<Job>(job =>
            {
                job.JobId = jobId;
                job.JobNumber = "002233";
            });

            Saga.Data.JobReferenceId = jobWithNoHomeOwner.JobId;
            Saga.Handle(new HomeBuyerDetailsResponse
            {
                FirstName = "NewHome",
                LastName = "Owner",
            });

            var homeOwnerAfterResponseProcessed = TestDatabase.Single<HomeOwner>("WHERE JobId = @0;", jobId);

            homeOwnerAfterResponseProcessed.HomeOwnerName.ShouldEqual("Owner, NewHome");
            homeOwnerAfterResponseProcessed.HomeOwnerNumber.ShouldEqual(1);
        }
    }
}