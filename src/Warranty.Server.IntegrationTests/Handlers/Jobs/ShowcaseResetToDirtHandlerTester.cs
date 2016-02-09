namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Core.Entities;
    using Core.Extensions;
    using NUnit.Framework;
    using Should;
    using TIPS.Events.JobEvents;

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

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void Task_should_be_deleted()
        {
            var task = Get<Task>(_task.TaskId);
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void Job_should_be_deleted()
        {
            var job = Get<Job>(_job.JobId);
        }
    }
}