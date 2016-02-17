using System;
using NUnit.Framework;
using Should;
using TIPS.Events.JobEvents;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    [TestFixture]
    public class ShowcaseElevationUpdateApprovedHandlerTester : HandlerTester<ShowcaseElevationUpdateApproved>
    {
        [Test]
        public void ShowcaseElevationUpdated_AssertUpdated()
        {
            var newElevation = "Z";
            var job = GetSaved<Job>();
            Send(new ShowcaseElevationUpdateApproved
            {
                JobNumber = job.JobNumber,
                Elevation = newElevation
            });

            using (TestDatabase)
            {
                var showcaseFromDb = TestDatabase.Single<Job>(string.Format("WHERE JobNumber = {0}", job.JobNumber));
                showcaseFromDb.Elevation.ShouldEqual(newElevation);
            }
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void ShowcaseElevationUpdated_NoJobNumberThrowsInvalidOperationException()
        {
            Send(new ShowcaseElevationUpdateApproved
            {
                Elevation = "Z"
            });
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void ShowcaseElevationUpdated_JobDoesntExistThrowsInvalidOperationException()
        {
            Send(new ShowcaseElevationUpdateApproved
            {
                Elevation = "Z",
                JobNumber = "fake"
            });
        }
    }
}
