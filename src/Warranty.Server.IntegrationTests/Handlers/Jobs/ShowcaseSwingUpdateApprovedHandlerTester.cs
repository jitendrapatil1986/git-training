using System;
using NUnit.Framework;
using Should;
using TIPS.Events.JobEvents;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    [TestFixture]
    public class ShowcaseSwingUpdateApprovedHandlerTester : HandlerTester<ShowcaseSwingUpdateApproved>
    {
        private RandomStringGenerator randomString = new RandomStringGenerator();
        private ShowcaseSwingUpdateApproved GetMessage(string jobNumber)
        {
            return new ShowcaseSwingUpdateApproved
            {
                JobNumber = jobNumber,
                Swing = randomString.Get(20)
            };
        }

        [Test]
        public void ShowcaseSwingUpdateApproved_AssertSwingUpdated()
        {
            var job = GetSaved<Job>();
            var message = GetMessage(job.JobNumber);

            Send(message);

            using (TestDatabase)
            {
                var jobFromDb = TestDatabase.SingleById<Job>(job.JobId);
                jobFromDb.Swing.ShouldEqual(message.Swing);
            }
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void ShowcaseSwingUpdateApproved_NoJobNumberThrowsInvalidOperationException()
        {
            var message = GetMessage(jobNumber: null);
            Send(message);
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void ShowcaseSwingUpdateApproved_MissingJobThrowsInvalidOperationException()
        {
            var message = GetMessage(jobNumber: "fake");
            Send(message);
        }
    }
}
