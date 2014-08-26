using Accounting.Events.Job;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    [TestFixture]
    public class JobElevationUpdatedHandlerTester : HandlerTester<JobElevationUpdated>
    {
        private Job _job;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _job = GetSaved<Job>();

            Send(x =>
            {
                x.JDEId = _job.JdeIdentifier;
                x.Elevation = "S";
            });
        }

        [Test]
        public void Job_Elevation_Should_Be_Updated()
        {
            var job = Get<Job>(_job.JobId);
            job.Elevation.ShouldEqual(Event.Elevation);
        }
    }
}