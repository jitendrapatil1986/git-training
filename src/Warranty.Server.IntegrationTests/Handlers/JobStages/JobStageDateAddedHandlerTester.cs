using System;
using System.Runtime.InteropServices;
using Accounting.Events.Job;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;
using Warranty.Server.Extensions;

namespace Warranty.Server.IntegrationTests.Handlers.JobStages
{
    [TestFixture]
    public class JobStageDateAddedHandlerTester : HandlerTester<JobStageDateAdded>
    {
        private Job _job;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _job = GetSaved<Job>();

            Send(x =>
            {
                x.JDEId = _job.JobNumber + "/003";
                x.Job = _job.JobNumber;
                x.JobStage = "3.0";
                x.ActualCompletionDate = new DateTime(2014, 1, 1);
            });
        }

        [Test]
        public void Job_Stage_Date_Should_Be_Added()
        {
            using (TestDatabase)
            {
                var jobStage = TestDatabase.SingleByJdeId<JobStage>(Event.JDEId);
                jobStage.CompletionDate.ShouldEqual(Event.ActualCompletionDate);
                jobStage.JobId.ShouldEqual(_job.JobId);
                jobStage.Stage.ShouldEqual(Convert.ToInt32(Convert.ToDecimal(Event.JobStage)));
            }
        }
    }
}