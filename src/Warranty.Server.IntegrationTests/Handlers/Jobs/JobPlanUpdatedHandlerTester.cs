using Accounting.Events.Job;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    [TestFixture]
    public class JobPlanUpdatedHandlerTester : HandlerTester<JobPlanUpdated>
    {
        private Job _job;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var community = GetSaved<Community>();
            _job = GetSaved<Job>(j => j.CommunityId = community.CommunityId);

            Send(x =>
            {
                x.JDEId = _job.JdeIdentifier;
                x.Plan = "1234";
            });
        }

        [Test]
        public void Job_Close_Date_Should_Be_Updated()
        {
            var job = Get<Job>(_job.JobId);
            job.PlanNumber.ShouldEqual(Event.Plan);
        }
    }
}