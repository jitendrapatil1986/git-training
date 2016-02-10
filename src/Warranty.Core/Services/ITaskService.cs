using System;
using System.Collections.Generic;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;

namespace Warranty.Core.Services
{
    public interface ITaskService
    {
        bool TaskExists(Guid jobId, TaskType taskType);

        void CreateTaskIfDoesntExist(Guid jobId, Guid wsrEmployeeId, TaskType taskType);

        void CreateTask(Guid jobId, Guid wsrEmployeeId, TaskType taskType);

        void DeleteTask(Guid jobId, TaskType taskType);
    }
}