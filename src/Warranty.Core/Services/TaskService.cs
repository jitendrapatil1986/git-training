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

        public TaskService(IDatabase database)
        {
            _database = database;
        }

        public bool TaskExists(Guid jobId, TaskType taskType)
        {
            using (_database)
            {
                return _database.Single<int>(string.Format("SELECT COUNT(1) FROM Tasks WHERE ReferenceId = '{0}' AND TaskType = {1}", jobId, taskType.Value)) >= 1;
            }
        }

        public void CreateTaskIfDoesntExist(Guid jobId, Guid wsrEmployeeId, TaskType taskType)
        {
            if (!TaskExists(jobId, taskType))
            {
                CreateTask(jobId, wsrEmployeeId, taskType);
            }
        }

        public void DeleteTask(Guid jobId, TaskType taskType)
        {
            _database.Delete(string.Format("DELETE FROM Tasks WHERE ReferenceId = '{0}' AND TaskType = {1}", jobId, taskType));
        }

        public void CreateTask(Guid jobId, Guid wsrEmployeeId, TaskType taskType)
        {
            using (_database)
            {
                var task = new Task
                {
                    EmployeeId = wsrEmployeeId,
                    TaskType = taskType,
                    Description = taskType.DisplayName,
                    ReferenceId = jobId
                };
                _database.Insert(task);
            }
        }
    }
}
