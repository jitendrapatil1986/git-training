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
                var employeeId = _database.ExecuteScalar<Guid>("Select EmployeeId from CommunityAssignments where CommunityId = @0", entity.CommunityId);
                if (employeeId != Guid.Empty && (entity.Stage == 3 || entity.Stage == 7 || entity.Stage == 10))
                {
                    var description = String.Empty;
                    var taskType = TaskType.JobStageChanged;

                    switch (entity.Stage)
                    {
                        case 3:
                            description = "Contact builder to coordinate a warranty introduction. Job # {0}";
                            taskType = TaskType.JobStage3;
                            break;
                        case 7:
                            description = "Contact builder to coordinate a 244 walk. Job # {0}";
                            taskType = TaskType.JobStage7;
                            break;
                        case 10:
                            description = "Contact homeowner to coordinate warranty orientation. Job # {0}";
                            taskType = TaskType.JobStage10;
                            break;
                    }

                    var task = new Task
                    {
                        EmployeeId = employeeId,
                        Description = String.Format(description, entity.JobNumber),
                        ReferenceId = entity.JobId,
                        TaskType = taskType,
                    };

                    _database.Insert(task);
                }
            }
        }
    }
}
