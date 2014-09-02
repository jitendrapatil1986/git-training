using System;
using System.Linq;
using Accounting.Events.Job;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;
using Warranty.Server.Extensions;

namespace Warranty.Server.IntegrationTests.Handlers.JobStages
{
    [TestFixture]
    public class JobStageDateUpdatedHandlerTester : HandlerTester<JobStageDateUpdated>
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var jobStage = GetSaved<JobStage>();
            
            Send(x =>
            {
                x.JDEId = jobStage.JdeIdentifier;
                x.ActualCompletionDate = new DateTime(2014, 1, 1);
            });
        }

        [Test]
        public void Job_CompletionDate_Should_Be_Updated()
        {
            using (TestDatabase)
            {
                var jobStage = TestDatabase.SingleByJdeId<JobStage>(Event.JDEId);
                jobStage.CompletionDate.ShouldEqual(Event.ActualCompletionDate);
            }
        }
    }
}