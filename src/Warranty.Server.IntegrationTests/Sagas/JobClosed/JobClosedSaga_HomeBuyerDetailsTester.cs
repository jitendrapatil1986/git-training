using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Fake.Bus;
using log4net;
using Moq;
using NServiceBus.Saga;
using NUnit.Framework;
using Should;
using TIPS.Commands.Responses;
using TIPS.Events.Models;
using Warranty.Core;
using Warranty.Core.Entities;
using Warranty.Core.Features.Homeowner;
using Warranty.Core.Features.Job;
using Warranty.Server.Sagas;
using Job = Warranty.Core.Entities.Job;

namespace Warranty.Server.IntegrationTests.Sagas.JobClosed
{
    [TestFixture]
    public class JobClosedSaga_HomeBuyerDetailsTester
    {
        [TestFixtureSetUp]
        public void SetupFixture()
        {
            Mapper.Initialize(c => c.AddProfile(new MappingProfile()));
        }

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
            Response = new HomeBuyerDetailsResponse
            {
                ContactId = Guid.NewGuid(),
                EmailAddresses = new List<Email> {new Email {Address = "test@mailinator.com"}},
                PhoneNumbers = new List<PhoneNumber> {new PhoneNumber {Number = "555-555-5555"}},
                FirstName = "John",
                LastName = "Doe"
            };
        }

        public HomeBuyerDetailsResponse Response { get; set; }

        public JobClosedSaga Saga { get; set; }

        public FakeBus Bus { get; set; }
        public Mock<IMediator> Mediator { get; set; }
        public Mock<ILog> Log { get; set; }

        [Test]
        public void WhenHomeOwnerDoesNotExists_ShouldCreateNewHomeOwner()
        {
            Mediator.Setup(m => m.Request(It.IsAny<GetJobQuery>())).Returns(new Job());
            Mediator.Setup(m => m.Send(It.IsAny<CreateNewHomeOwnerCommand>())).Returns(new HomeOwner());
            Saga.Data.JobId = Guid.NewGuid();

            Saga.Handle(Response);

            Mediator.Verify(m => m.Send(It.IsAny<CreateNewHomeOwnerCommand>()), Times.Once);
            Mediator.Verify(m => m.Send(It.IsAny<AssignHomeOwnerToJobCommand>()), Times.Once);
            Bus.SentLocalMessages.OfType<JobClosedSaga_CloseJob>().Count().ShouldEqual(1);
        }

        [Test]
        public void WhenHomeOwnerExcists_ShouldOnlyUpdateHomeOwner()
        {
            Mediator.Setup(m => m.Request(It.IsAny<GetJobQuery>())).Returns(new Job());
            Mediator.Setup(m => m.Request(It.IsAny<GetHomeOwnerQuery>())).Returns(new HomeOwner());
            Saga.Data.JobId = Guid.NewGuid();

            Saga.Handle(Response);

            Mediator.Verify(m => m.Send(It.IsAny<CreateNewHomeOwnerCommand>()), Times.Never);
            Mediator.Verify(m => m.Send(It.IsAny<AssignHomeOwnerToJobCommand>()), Times.Once);
            Bus.SentLocalMessages.OfType<JobClosedSaga_CloseJob>().Count().ShouldEqual(1);
        }
    }
}