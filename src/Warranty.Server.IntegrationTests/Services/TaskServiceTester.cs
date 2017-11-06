using System;
using System.Linq;
using NUnit.Framework;
using Should;
using StructureMap;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;
using Warranty.Core.Services;
using Task = Warranty.Core.Entities.Task;

namespace Warranty.Server.IntegrationTests.Services
{
    [TestFixture]
    public class TaskServiceTester : ServiceTesterBase
    {
        private ITaskService _taskService;

        public TaskServiceTester()
        {
            _taskService = ObjectFactory.GetInstance<ITaskService>();
        }

        private Task GetTask(Guid jobId, TaskType taskType)
        {
            return TestDatabase.SingleOrDefault<Task>(string.Format("WHERE ReferenceId = '{0}' AND TaskType = {1}", jobId, taskType.Value));
        }

        private void AssertTaskIsCorrect(Task task, Job job, Employee wsr, TaskType taskType)
        {
            task.ShouldNotBeNull();
            task.ReferenceId.ShouldEqual(job.JobId);
            task.Description.ShouldEqual(taskType.DisplayName);
            task.TaskType.ShouldEqual(taskType);
            task.EmployeeId.ShouldEqual(wsr.EmployeeId);
        }

        [Test]
        public void Check_CreateTaskIfDoesntExist_CreatesTaskIfDoesntExist()
        {
            var taskType = TaskType.QualityIntroductionOfWSR;
            var community = Get<Community>();
            var job = Get<Job>(j => j.CommunityId = community.CommunityId);
            var wsr = Get<Employee>();
            using (TestDatabase)
            {
                TestDatabase.Insert(new CommunityAssignment
                {
                    CommunityId = community.CommunityId,
                    EmployeeId = wsr.EmployeeId
                });
            }
            GetTask(job.JobId, taskType).ShouldBeNull();

            _taskService.CreateTaskUnlessExists(job.JobId, taskType);

            AssertTaskIsCorrect(GetTask(job.JobId, taskType), job, wsr, taskType);
        }

        [Test]
        public void Check_CreateTaskIfDoesntExist_DoesntCreateTaskIfExists()
        {
            var taskType = TaskType.QualityIntroductionOfWSR;
            var community = Get<Community>();
            var job = Get<Job>(j => j.CommunityId = community.CommunityId);
            var wsr = Get<Employee>();
            using (TestDatabase)
            {
                TestDatabase.Insert(new CommunityAssignment
                {
                    CommunityId = community.CommunityId,
                    EmployeeId = wsr.EmployeeId
                });
            }

            GetTask(job.JobId, taskType).ShouldBeNull();

            _taskService.CreateTaskUnlessExists(job.JobId, taskType);
            var task = GetTask(job.JobId, taskType);
            AssertTaskIsCorrect(task, job, wsr, taskType);

            _taskService.CreateTaskUnlessExists(job.JobId, taskType);
            var allTasks = _taskService.GetTasksByJobId(job.JobId);
            allTasks.Count.ShouldEqual(1);
            AssertTaskIsCorrect(allTasks.First(), job, wsr, taskType);
        }
    }
}
