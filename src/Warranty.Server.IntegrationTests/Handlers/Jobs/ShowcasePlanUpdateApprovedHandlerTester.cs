using System;
using System.Text;
using NUnit.Framework;
using Should;
using TIPS.Events.JobEvents;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    public class ShowcasePlanUpdateApprovedHandlerTester : HandlerTester<ShowcasePlanUpdateApproved>
    {
        private RandomStringGenerator randomString = new RandomStringGenerator();
        private ShowcasePlanUpdateApproved GetMessage(string jobNumber)
        {
            return new ShowcasePlanUpdateApproved
            {
                JobNumber = jobNumber,
                Elevation = randomString.Get(1),
                PlanName = randomString.Get(30),
                PlanNumber = randomString.Get(4)
            };
        }

        [Test]
        public void ShowcasePlanUpdateApproved_AssertUpdated()
        {

            var job = GetSaved<Job>();
            var message = GetMessage(job.JobNumber);

            Send(message);

            using (TestDatabase)
            {
                var showcaseFromDb = TestDatabase.Single<Job>(string.Format("WHERE JobNumber = {0}", job.JobNumber));

                showcaseFromDb.Elevation.ShouldEqual(message.Elevation);
                showcaseFromDb.PlanName.ShouldEqual(message.PlanName);
                showcaseFromDb.PlanNumber.ShouldEqual(message.PlanNumber);
            }
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void ShowcasePlanUpdateApproved_NoJobNumberThrowsInvalidOperationException()
        {
            var message = GetMessage(jobNumber: null);
            Send(message);
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void ShowcasePlanUpdateApproved_JobDoesntExistThrowsInvalidOperationException()
        {
            var message = GetMessage(jobNumber: "fake");
            Send(message);
        }
    }
}
