using System;
using System.Collections.Generic;
using Construction.Events.Jobs;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;

namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    [TestFixture]
    public class JobStageUpdatedHandlerTester : HandlerTester<JobStageUpdated>
    {
        private Job _job;
        private Community _community;
        private Employee _wsr;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _community = GetSaved<Community>();
            _job = GetSaved<Job>(j => j.CommunityId = _community.CommunityId);
            _wsr = GetSaved<Employee>();
            using (TestDatabase)
            {
                TestDatabase.Insert(new CommunityAssignment
                {
                    CommunityId = _community.CommunityId,
                    EmployeeId = _wsr.EmployeeId
                });
            }
            Send(x =>
            {
                x.Job = _job.JdeIdentifier;
                x.Stage = Convert.ToInt32(Convert.ToDecimal("5.0"));
            });
        }

        [Test]
        public void Job_Close_Date_Should_Be_Updated()
        {
            var job = Get<Job>(_job.JobId);
            job.Stage.ShouldEqual(Convert.ToInt32(Convert.ToDecimal(Event.Stage)));
        }

        private void UpdateToStageAssertTasksExist(int stage, Action<List<Task>> assertTasksAction, bool showcase = false)
        {
            var job = GetSaved<Job>(x =>
            {
                x.CommunityId = _community.CommunityId;
                x.Stage = 0;
            });
            if (!showcase)
            {
                var homeowner = GetSaved<HomeOwner>(h => h.JobId = job.JobId);
                job.CurrentHomeOwnerId = homeowner.HomeOwnerId;
                using (TestDatabase)
                {
                    TestDatabase.Update(job);
                }
            }
            Send(x =>
            {
                x.Job = job.JdeIdentifier;
                x.Stage = stage;
            });
            using (TestDatabase)
            {
                var tasks = TestDatabase.Fetch<Task>(string.Format("WHERE ReferenceId = '{0}'", job.JobId));
                assertTasksAction(tasks);
            }
        }

        [Test]
        public void Job_Stage_3_Should_Create_ToDo()
        {
            UpdateToStageAssertTasksExist(3, tasks =>
            {
                tasks.Count.ShouldEqual(1);
                tasks[0].TaskType.ShouldEqual(TaskType.JobStage3);
            });
        }

        [Test]
        public void Job_Stage_7_Should_Create_ToDo()
        {
            UpdateToStageAssertTasksExist(7, tasks =>
            {
                tasks.Count.ShouldEqual(1);
                tasks[0].TaskType.ShouldEqual(TaskType.JobStage7);
            });
        }

        [Test]
        public void Job_Stage_10_Should_Create_ToDo()
        {
            UpdateToStageAssertTasksExist(10, tasks =>
            {
                tasks.Count.ShouldEqual(1);
                tasks[0].TaskType.ShouldEqual(TaskType.JobStage10);
            });
        }

        [Test]
        public void Showcase_Stage_7_Should_Create_ToDo()
        {
            UpdateToStageAssertTasksExist(7, tasks =>
            {
                tasks.Count.ShouldEqual(1);
                tasks[0].TaskType.ShouldEqual(TaskType.JobStage7);
            }, showcase: true);
        }

        [Test]
        public void Showcase_Stage_3_Should_Not_Create_ToDo()
        {
            UpdateToStageAssertTasksExist(3, tasks =>
            {
                tasks.Count.ShouldEqual(0);
            }, showcase: true);
        }

        [Test]
        public void Showcase_Stage_10_Should_Not_Create_ToDo()
        {
            UpdateToStageAssertTasksExist(10, tasks =>
            {
                tasks.Count.ShouldEqual(0);
            }, showcase: true);
        }
    }
}