using System;
using Accounting.Events.Job;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;

namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    [TestFixture]
    public class JobClosedHandlerWithHomeOwnerTester : HandlerTester<JobClosed>
    {
        private Job _job;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _job = GetSaved<Job>(job =>
            {
                var homeOwner = GetSaved<HomeOwner>();
                job.CurrentHomeOwnerId = homeOwner.HomeOwnerId;
                job.Stage = 10;

                var employee = GetSaved<Employee>();
                GetSaved<CommunityAssignment>(commAssign =>
                {
                    commAssign.EmployeeId = employee.EmployeeId;
                    commAssign.CommunityId = job.CommunityId;
                });
            });

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

        [Test]
        public void When_A_Job_Close_Date_Is_Entered_A_Warranty_Orientation_ToDo_Task_Should_Be_Added()
        {
            var task = TestDatabase.Single<Task>("SELECT * FROM dbo.Tasks WHERE ReferenceId = @0;", _job.JobId);

            task.TaskType.ShouldEqual(TaskType.JobStage10JobClosed);
            task.Description.ShouldEqual(TaskType.JobStage10JobClosed.DisplayName);
        }
    }
}