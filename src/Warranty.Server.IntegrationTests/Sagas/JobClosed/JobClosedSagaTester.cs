using System;
using System.Linq;
using Fake.Bus;
using log4net;
using Moq;
using NUnit.Framework;
using Should;
using TIPS.Commands.Requests;
using Warranty.Core;
using Warranty.Core.Entities;
using Warranty.Core.Features.Homeowner;
using Warranty.Core.Features.Job;
using Warranty.Server.Sagas;

namespace Warranty.Server.IntegrationTests.Sagas.JobClosed
{
    [TestFixture]
    public class JobClosedSagaTester
    {
        [SetUp]
        public void Setup()
        {
            Log = new Mock<ILog>();
            Mediator = new Mock<IMediator>();
            Bus = new FakeBus();
            Saga = new JobClosedSaga(Log.Object, Mediator.Object)
            {
                Data = new JobClosedSagaData(),
                Bus = Bus
            };
        }

        public JobClosedSaga Saga { get; set; }

        public FakeBus Bus { get; set; }
        public Mock<IMediator> Mediator { get; set; }
        public Mock<ILog> Log { get; set; }

        [Test]
        public void WhenJobNotFound_ShouldExitWithError()
        {
            Mediator.Setup(m => m.Request(It.IsAny<GetJobQuery>())).Returns(null as Job);
            var closeDate = DateTime.UtcNow;

            var message = new Accounting.Events.Job.JobClosed
            {
                Job = "2873283782723",
                CloseDate = closeDate
            };

            Saga.Handle(message);
            Saga.Data.JobNumber.ShouldNotBeNull();
            Saga.Data.CloseDate.ShouldEqual(closeDate);
            Bus.SentMessages<RequestHomeBuyerDetails>().Any().ShouldBeFalse();
            Bus.SentLocalMessages.Any().ShouldBeFalse();
            Log.Verify(m => m.ErrorFormat(It.IsAny<string>(), message.Job), Times.AtLeastOnce, "Expected error message to be logged with job number");
        }

        [Test]
        public void WhenCurrentHomeOwnerNotSet_ShouldRequestFromTips()
        {
            var jobNumber = "74645372";
            Mediator.Setup(m => m.Request(It.IsAny<GetJobQuery>())).Returns(
                new Job
                {
                    JobId = Guid.NewGuid(),
                    JobNumber = jobNumber
                }
            );
            Mediator.Setup(m => m.Request(It.IsAny<GetHomeOwnerQuery>())).Returns(null as HomeOwner);

            var message = new Accounting.Events.Job.JobClosed
            {
                Job = jobNumber
            };

            Saga.Handle(message);
            Saga.Data.JobId.ShouldNotEqual(Guid.Empty);
            Bus.SentLocalMessages.Any().ShouldBeFalse();
            Bus.SentMessages<RequestHomeBuyerDetails>().Count().ShouldEqual(1);
        }

        [Test]
        public void WhenCurrentHomeOwnerNotFound_ShouldRequestFromTips()
        {
            var jobNumber = "74645372";
            Mediator.Setup(m => m.Request(It.IsAny<GetJobQuery>())).Returns(
                new Job
                {
                    JobId = Guid.NewGuid(),
                    CurrentHomeOwnerId = Guid.NewGuid(),
                    JobNumber = jobNumber
                }
            );
            Mediator.Setup(m => m.Request(It.IsAny<GetHomeOwnerQuery>())).Returns(null as HomeOwner);

            var message = new Accounting.Events.Job.JobClosed
            {
                Job = jobNumber
            };

            Saga.Handle(message);
            Saga.Data.JobId.ShouldNotEqual(Guid.Empty);
            Saga.Data.CurrentHomeownerId.ShouldNotEqual(Guid.Empty);
            Bus.SentLocalMessages.Any().ShouldBeFalse();
            Bus.SentMessages<RequestHomeBuyerDetails>().Count().ShouldEqual(1);
        }

        [Test]
        public void WhenHomeOwnerInfoGood_ShouldSendToCloseJobStep()
        {
            var jobNumber = "12654637";
            var homeOwnerId = Guid.NewGuid();
            Mediator.Setup(m => m.Request(It.IsAny<GetJobQuery>())).Returns(
                new Job
                {
                    JobId = Guid.NewGuid(),
                    CurrentHomeOwnerId = Guid.NewGuid(),
                    JobNumber = jobNumber
                }
            );
            Mediator.Setup(m => m.Request(It.IsAny<GetHomeOwnerQuery>())).Returns(
                new HomeOwner
                {
                    HomeOwnerId = homeOwnerId
                });

            var message = new Accounting.Events.Job.JobClosed
            {
                Job = jobNumber
            };

            Saga.Handle(message);
            Saga.Data.JobId.ShouldNotEqual(Guid.Empty);
            Saga.Data.CurrentHomeownerId.ShouldNotEqual(Guid.Empty);
            Bus.SentMessages<RequestHomeBuyerDetails>().Count().ShouldEqual(0);
            Bus.SentLocalMessages.OfType<JobClosedSaga_CloseJob>().Count().ShouldEqual(1);
            
        }
    }
}