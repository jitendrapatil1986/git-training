using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private List<Task> GetTasksByJobId(Guid jobId)
        {
            return TestDatabase.Fetch<Task>(string.Format("WHERE ReferenceId = '{0}'", jobId));
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
        public void Check_CreateTask()
        {
            var taskType = TaskType.JobStage3;
            var job = Get<Job>();
            var wsr = Get<Employee>();

            _taskService.CreateTask(job.JobId, wsr.EmployeeId, taskType);

            var task = GetTask(job.JobId, taskType);

            AssertTaskIsCorrect(task, job, wsr, taskType);
        }

        [Test]
        public void Check_CreateTaskIfDoesntExist_CreatesTaskIfDoesntExist()
        {
            var taskType = TaskType.JobStage3;
            var job = Get<Job>();
            var wsr = Get<Employee>();
            GetTask(job.JobId, taskType).ShouldBeNull();

            _taskService.CreateTaskIfDoesntExist(job.JobId, wsr.EmployeeId, taskType);

            AssertTaskIsCorrect(GetTask(job.JobId, taskType), job, wsr, taskType);
        }

        [Test]
        public void Check_CreateTaskIfDoesntExist_DoesntCreateTaskIfExists()
        {
            var taskType = TaskType.JobStage3;
            var job = Get<Job>();
            var wsr = Get<Employee>();
            GetTask(job.JobId, taskType).ShouldBeNull();

            _taskService.CreateTaskIfDoesntExist(job.JobId, wsr.EmployeeId, taskType);
            var task = GetTask(job.JobId, taskType);
            AssertTaskIsCorrect(task, job, wsr, taskType);

            _taskService.CreateTaskIfDoesntExist(job.JobId, wsr.EmployeeId, taskType);
            var allTasks = GetTasksByJobId(job.JobId);
            allTasks.Count.ShouldEqual(1);
            AssertTaskIsCorrect(allTasks.First(), job, wsr, taskType);
        }
    }
}
