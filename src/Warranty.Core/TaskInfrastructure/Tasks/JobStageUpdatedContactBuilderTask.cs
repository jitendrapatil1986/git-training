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
                            description = "Job is at Stage 3 - time to schedule a warranty introduction.";
                            taskType = TaskType.JobStage3;
                            break;
                        case 7:
                            description = "Job is at Stage 7 - time to schedule a 244 walk.";
                            taskType = TaskType.JobStage7;
                            break;
                        case 10:
                            description = "Job is at Stage 10 - time to schedule a warranty orientation.";
                            taskType = TaskType.JobStage10;
                            break;
                    }

                    var task = new Task
                    {
                        EmployeeId = employeeId,
                        Description = description,
                        ReferenceId = entity.JobId,
                        TaskType = taskType,
                    };

                    _database.Insert(task);
                }
            }
        }
    }
}
