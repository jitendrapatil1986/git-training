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
            _job = GetSaved<Job>();

            Send(x =>
            {
                x.JDEId = _job.JdeIdentifier;
                x.Plan = "1234";
            });
        }

        [Test]
        public void Job_Close_Date_Should_Be_Updated()
        {
            var payment = Get<Job>(_job.JobId);
            payment.PlanNumber.ShouldEqual(Event.Plan);
        }
    }
}