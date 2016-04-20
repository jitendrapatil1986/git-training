using Should;

namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    using System;
    using Core.Entities;
    using NUnit.Framework;
    using TIPS.Events.JobEvents;

    [TestFixture]
    public class ShowcaseResetToDirtHandlerTester : HandlerTester<ShowcaseResetToDirt>
    {
        private Job _job;
        private Task _task;
        private JobStage _stage;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _job = GetSaved<Job>();
            _task = GetSaved<Task>(t => t.ReferenceId = _job.JobId);
            _stage = GetSaved<JobStage>(s => s.JobId = _job.JobId);

            Send(x =>
            {
                x.JobNumber = _job.JobNumber;
            });
        }

        [Test]
        public void Should_delete_task()
        {
            TestDatabase.SingleOrDefaultById<Task>(_task.TaskId).ShouldBeNull();
        }

        [Test]
        public void Should_delete_job()
        {
            TestDatabase.SingleOrDefaultById<Job>(_job.JobId).ShouldBeNull();
        }

        [Test]
        public void Should_delete_stage()
        {
            TestDatabase.Fetch<JobStage>("WHERE JobId = @0", _job.JobId).ShouldBeEmpty();
        }

    }
}