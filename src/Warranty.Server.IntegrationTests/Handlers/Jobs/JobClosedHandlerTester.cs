using System;
using Accounting.Events.Job;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    [TestFixture]
    public class JobClosedHandlerTester : HandlerTester<JobClosed>
    {
        private Job _job;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _job = GetSaved<Job>();

            Send(x =>
            {
                x.JDEId = _job.JdeIdentifier;
                x.CloseDate = new DateTime(2015, 1, 1);
            });
        }

        [Test]
        public void Job_Close_Date_Should_Be_Updated()
        {
            var job = Get<Job>(_job.JobId);
            job.CloseDate.ShouldEqual(Event.CloseDate);
        }
    }
}