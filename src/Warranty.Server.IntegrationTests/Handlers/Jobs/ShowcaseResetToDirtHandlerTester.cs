namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    using Core.Entities;
    using NUnit.Framework;
    using TIPS.Events.JobEvents;
    using Should;

    [TestFixture]
    public class ShowcaseResetToDirtHandlerTester : HandlerTester<ShowcaseResetToDirt>
    {
        private Job _job;
        private Task _task;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _job = GetSaved<Job>();
            _task = GetSaved<Task>(t => t.ReferenceId = _job.JobId);

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
    }
}