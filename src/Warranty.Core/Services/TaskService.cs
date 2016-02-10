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
    public interface ITaskService
    {
        void CreateTask(Guid jobId, Guid wsrEmployeeId, TaskType taskType);

        List<Task> GetTasksByJobNumber(string jobNumber);

        List<Task> GetTasksByCommunityNumber(string communityNumber);
    }

    public class TaskService : ITaskService
    {
        private IDatabase _database;

        public TaskService(IDatabase database)
        {
            _database = database;
        }

        public void CreateTask(Guid jobId, Guid wsrEmployeeId, TaskType taskType)
        {
            using (_database)
            {
                var taskExists = _database.Single<int>(string.Format("SELECT COUNT(1) FROM Tasks WHERE ReferenceId = '{0}' AND TaskType = {1}", jobId, taskType)) >= 1;
                if(taskExists)
                    throw new Exception(string.Format("Task of type '{0}' already exists on job '{1}'", taskType, jobId));

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

        public List<Task> GetTasksByJobNumber(string jobNumber)
        {
            const string sql = @"SELECT 
                                    T.TaskId,
                                    T.EmployeeId,
                                    T.ReferenceId,
                                    T.Description,
                                    T.IsComplete,
                                    T.TaskType,
                                    T.CreatedDate,
                                    T.CreatedBy,
                                    T.UpdatedDate,
                                    T.UpdatedBy,
                                    T.IsNoAction
                                FROM 
                                    Tasks T 
                                    INNER JOIN Jobs J ON T.ReferenceId = J.JobId
                                WHERE 
                                    J.JobNumber = {0}";
            return _database.Fetch<Task>(string.Format(sql, jobNumber));
        }

        public List<Task> GetTasksByCommunityNumber(string communityNumber)
        {
            const string sql = @"SELECT 
                                    T.TaskId,
                                    T.EmployeeId,
                                    T.ReferenceId,
                                    T.Description,
                                    T.IsComplete,
                                    T.TaskType,
                                    T.CreatedDate,
                                    T.CreatedBy,
                                    T.UpdatedDate,
                                    T.UpdatedBy,
                                    T.IsNoAction
                                FROM Tasks T 
                                    INNER JOIN Jobs J
                                        ON T.ReferenceId = J.JobId
                                    INNER JOIN Communities C 
                                        ON J.CommunityId = C.CommunityId
                                    INNER JOIN Employees E
                                        ON E.EmployeeId = T.EmployeeId
                                WHERE 
                                    C.CommunityNumber = {0}";
            return _database.Fetch<Task>(string.Format(sql, communityNumber));
        } 
    }
}
