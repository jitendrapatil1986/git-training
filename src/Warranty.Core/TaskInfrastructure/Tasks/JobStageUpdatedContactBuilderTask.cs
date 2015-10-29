namespace Warranty.Core.TaskInfrastructure.Tasks
{
    using System;
    using Entities;
    using Enumerations;
    using Interfaces;
    using NPoco;

    public class JobStageUpdatedContactBuilderTask : ITask<Job>
    {
        private readonly IDatabase _database;

        public JobStageUpdatedContactBuilderTask(IDatabase database)
        {
            _database = database;
        }

        public void Create(Job entity)
        {
            using (_database)
            {
                if (entity.Stage == 3 || entity.Stage == 7 || entity.Stage == 10)
                {
                    var employeeId = _database.ExecuteScalar<Guid>("Select EmployeeId from CommunityAssignments where CommunityId = @0", entity.CommunityId);
                    if (employeeId == Guid.Empty)
                        throw new Exception("Employee not found");


                    var description = String.Empty;
                    var taskType = TaskType.JobStageChanged;

                    switch (entity.Stage)
                    {
                        case 3:
                            description = TaskType.JobStage3.DisplayName;
                            taskType = TaskType.JobStage3;
                            break;
                        case 7:
                            description = TaskType.JobStage7.DisplayName;
                            taskType = TaskType.JobStage7;
                            break;
                        case 10:
                            description = TaskType.JobStage10.DisplayName;
                            taskType = TaskType.JobStage10;
                            break;
                    }


                    var taskId =
                        _database.ExecuteScalar<Guid>(
                            "SELECT TaskId FROM Tasks WHERE ReferenceId = @0 AND TaskType = @1", entity.JobId,
                            taskType.Value);

                    if (taskId == Guid.Empty)
                    {
                        var task = new Task
                        {
                            EmployeeId = employeeId,
                            Description = description,
                            ReferenceId = entity.JobId,
                            TaskType = taskType
                        };

                        _database.Insert(task);
                    }

                }
            }
        }
    }
}
