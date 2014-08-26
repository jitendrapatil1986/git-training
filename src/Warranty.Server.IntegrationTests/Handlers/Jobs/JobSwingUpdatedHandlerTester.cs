using Accounting.Events.Job;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    [TestFixture]
    public class JobSwingUpdatedHandlerTester : HandlerTester<JobSwingUpdated>
    {
        private Job _job;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _job = GetSaved<Job>();

            Send(x =>
            {
                x.JDEId = _job.JdeIdentifier;
                x.Swing = "N";
            });
        }

        [Test]
        public void Job_Swing_Should_Be_Updated()
        {
            var payment = Get<Job>(_job.JobId);
            payment.Swing.ShouldEqual(Event.Swing);
        }
    }
}