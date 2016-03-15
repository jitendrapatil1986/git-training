using System;
using NUnit.Framework;
using Should;
using Moq;
using Land.Events;
using Warranty.Core.Entities;
using Warranty.Core.Services;
using Warranty.Server.Handlers.Jobs;


namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    [TestFixture]
    public class JobAddressChangedHandlerTests : HandlerTester<LotAddressChanged>
    {
        [Test]
        public void Constructor_WhenJobServiceIsNull_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new JobAddressChangedHandler(null));

            exception.ParamName.ShouldEqual("jobService");
        }

        [Test]
        public void Handle_WhenJdeIdentifierDoesNotMatchJob_DoesNotSaveJob()
        {
            var jobService = new Mock<IJobService>();
            var handler = new JobAddressChangedHandler(jobService.Object);
            var message = new LotAddressChanged
            {
                JdeIdentifier = "some value"
            };

            handler.Handle(message);

            jobService.Verify(x => x.GetJobByNumber(message.JdeIdentifier), Times.Once);
            jobService.Verify(x => x.Save(It.IsAny<Job>()), Times.Never);
        }

        [Test]
        public void Handle_WhenJdeIdenitfierMatchesJob_UpdatesJobAddress()
        {
            const string originalAddress = "9999 deadbeef";
            var job = new Job
            {
                AddressLine = originalAddress
            };
            var jobService = new Mock<IJobService>();
            jobService.Setup(x => x.GetJobByNumber(It.IsAny<string>()))
                      .Returns(job);
            var handler = new JobAddressChangedHandler(jobService.Object);
            var message = new LotAddressChanged
            {
                JdeIdentifier = "some value",
                NewStreetNumber = "1234",
                NewStreetName = "some street"
            };

            handler.Handle(message);

            job.AddressLine.ShouldNotEqual(originalAddress);
            job.AddressLine.ShouldContain(message.NewStreetNumber);
            job.AddressLine.ShouldContain(message.NewStreetName);
        }

        [Test]
        public void Handle_WhenJdeIdenitfierMatchesJob_SavesJob()
        {
            var job = new Job();
            var jobService = new Mock<IJobService>();
            jobService.Setup(x => x.GetJobByNumber(It.IsAny<string>()))
                      .Returns(job);
            var handler = new JobAddressChangedHandler(jobService.Object);
            var message = new LotAddressChanged
            {
                JdeIdentifier = "some value",
                NewStreetNumber = "1234",
                NewStreetName = "some street"
            };

            handler.Handle(message);

            jobService.Verify(x => x.Save(job), Times.Once);
        }

        [TestCase("   1234", "1234")]
        [TestCase("4321   ", "4321")]
        [TestCase("   8794    ", "8794")]
        public void Handle_WhenJdeIdentifierContainsLeadingOrTrailingWhiteSpace_ProvidesJdeIdentifierWithoutWhiteSpaceToJobService(string jdeIdentifier, string expectedJobNumber)
        {
            var jobService = new Mock<IJobService>();
            var handler = new JobAddressChangedHandler(jobService.Object);
            var message = new LotAddressChanged
            {
                JdeIdentifier = jdeIdentifier,
                NewStreetNumber = "1234",
                NewStreetName = "some street"
            };

            handler.Handle(message);

            jobService.Verify(x => x.GetJobByNumber(expectedJobNumber), Times.Once);
        }
    }
}