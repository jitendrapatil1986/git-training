using System;
using System.Collections.Generic;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;

namespace Warranty.Core.Services
{
    public interface ITaskService
    {
        bool TaskExists(Guid jobId, TaskType taskType);

        void CreateTaskUnlessExists(Guid jobId, TaskType taskType);

        void DeleteTask(Guid jobId, TaskType taskType);

        List<Task> GetTasksByJobId(Guid jobId);

        void CreateTasks(Guid jobId);
    }
}