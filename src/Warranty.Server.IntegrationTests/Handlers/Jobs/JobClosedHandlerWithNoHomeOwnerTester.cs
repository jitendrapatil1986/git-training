using System;
using Accounting.Events.Job;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    public class JobClosedHandlerWithNoHomeOwnerTester : HandlerTester<JobClosed>
    {
        private Job _job;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _job = GetSaved<Job>(job =>
            {
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
        public void When_A_Job_Close_Date_Is_Entered_Without_A_Home_Owner_A_Warranty_Orientation_ToDo_Task_Should_Not_Be_Added()
        {
            var task = TestDatabase.Fetch<Task>("SELECT * FROM dbo.Tasks WHERE ReferenceId = @0;", _job.JobId);

            task.Count.ShouldEqual(0);
        }
    }
}