using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPoco;
using NPoco.Expressions;
using Warranty.Core.Enumerations;
using Task = Warranty.Core.Entities.Task;
using Warranty.Core.Extensions;

namespace Warranty.Core.Services
{

    public class TaskService : ITaskService
    {
        private IDatabase _database;
        private IEmployeeService _employeeService;
        private IJobService _jobService;

        public TaskService(IDatabase database, IEmployeeService employeeService, IJobService jobService)
        {
            if (database == null)
                throw new ArgumentNullException("database");
            
            if (employeeService == null)
                throw new ArgumentNullException("employeeService");
            
            if (jobService == null)
                throw new ArgumentNullException("jobService");

            _database = database;
            _employeeService = employeeService;
            _jobService = jobService;
        }

        public bool TaskExists(Guid jobId, TaskType taskType)
        {
            if (taskType == null)
                throw new ArgumentNullException("taskType");
            using (_database)
            {
                return _database.Single<int>(string.Format("SELECT COUNT(1) FROM Tasks WHERE ReferenceId = '{0}' AND TaskType = {1}", jobId, taskType.Value)) >= 1;
            }
        }

        public void CreateTaskUnlessExists(Guid jobId, TaskType taskType)
        {
            if (taskType == null)
                throw new ArgumentNullException("taskType");
            if (!TaskExists(jobId, taskType))
            {
                CreateTask(jobId, taskType);
            }
        }

        public void DeleteTask(Guid jobId, TaskType taskType)
        {
            if (taskType == null)
                throw new ArgumentNullException("taskType");
            _database.Execute(string.Format("DELETE FROM Tasks WHERE ReferenceId = '{0}' AND TaskType = {1}", jobId, taskType.Value));
        }

        public List<Task> GetTasksByJobId(Guid jobId)
        {
            return _database.Fetch<Task>(string.Format("WHERE ReferenceId = '{0}'", jobId));
        }

        public void CreateTasks(Guid jobId)
        {
            var job = _jobService.GetJobById(jobId);
            var isModelOrShowcase = _jobService.IsModelOrShowcase(job);
            var validTaskTypes = ValidTaskTypes(isModelOrShowcase);

            validTaskTypes
                .Where(type => type.Stage <= job.Stage)
                .ForEach(type =>
                    CreateTaskUnlessExists(jobId, type)
                );
        }

        private IEnumerable<TaskType> ValidTaskTypes(bool isModelOrShowcase)
        {
            if (isModelOrShowcase)
                return TaskType.ValidModelOrShowcaseTasks;

            return TaskType.ValidJobWithHomeOwnerTasks;
        }

        private void CreateTask(Guid jobId, TaskType taskType)
        {
            if (taskType == null)
                throw new ArgumentNullException("taskType");
            var wsr = _employeeService.GetWsrByJobId(jobId);
            if (wsr == null) 
                throw new InvalidOperationException("Cannot create a task without a WSR assigned to the community.");

            using (_database)
            {
                var task = new Task
                {
                    EmployeeId = wsr.EmployeeId,
                    TaskType = taskType,
                    Description = taskType.DisplayName,
                    ReferenceId = jobId
                };
                _database.Insert(task);
            }
        }
    }
}
