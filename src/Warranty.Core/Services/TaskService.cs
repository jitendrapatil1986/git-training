using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPoco;
using Warranty.Core.Enumerations;
using Task = Warranty.Core.Entities.Task;

namespace Warranty.Core.Services
{

    public class TaskService : ITaskService
    {
        private IDatabase _database;
        private IEmployeeService _employeeService;

        public TaskService(IDatabase database, IEmployeeService employeeService)
        {
            _database = database;
            _employeeService = employeeService;
        }

        public bool TaskExists(Guid jobId, TaskType taskType)
        {
            using (_database)
            {
                return _database.Single<int>(string.Format("SELECT COUNT(1) FROM Tasks WHERE ReferenceId = '{0}' AND TaskType = {1}", jobId, taskType.Value)) >= 1;
            }
        }

        public void CreateTaskUnlessExists(Guid jobId, TaskType taskType)
        {
            if (!TaskExists(jobId, taskType))
            {
                CreateTask(jobId, taskType);
            }
        }

        public void DeleteTask(Guid jobId, TaskType taskType)
        {
            _database.Execute(string.Format("DELETE FROM Tasks WHERE ReferenceId = '{0}' AND TaskType = {1}", jobId, taskType.Value));
        }

        public List<Task> GetTasksByJobId(Guid jobId)
        {
            return _database.Fetch<Task>(string.Format("WHERE ReferenceId = '{0}'", jobId));
        }

        public void CreateTask(Guid jobId, TaskType taskType)
        {

            var wsr = _employeeService.GetWsrByJobId(jobId);

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
