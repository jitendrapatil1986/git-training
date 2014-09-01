using System;
using Accounting.Events.Job;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    [TestFixture]
    public class JobStageUpdatedHandlerTester : HandlerTester<JobStageUpdated>
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
                x.Stage = "5.0";
            });
        }

        [Test]
        public void Job_Close_Date_Should_Be_Updated()
        {
            var job = Get<Job>(_job.JobId);
            job.Stage.ShouldEqual(Convert.ToInt32(Convert.ToDecimal(Event.Stage)));
        }
    }
}