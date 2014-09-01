using System;
using Accounting.Events.Job;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers.JobStages
{
    [TestFixture]
    public class JobStageDateUpdatedHandlerTester : HandlerTester<JobStageDateUpdated>
    {
        private JobStage _jobStage;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _jobStage = GetSaved<JobStage>();

            Send(x =>
            {
                x.JDEId = _jobStage.JdeIdentifier;
                x.ActualCompletionDate = new DateTime(2014, 1, 1);
            });
        }

        [Test]
        public void Job_CompletionDate_Should_Be_Updated()
        {
            var jobStage = Get<JobStage>(new object[]{_jobStage.JobId, _jobStage.Stage});
            jobStage.CompletionDate.ShouldEqual(Event.ActualCompletionDate);
        }
    }
}