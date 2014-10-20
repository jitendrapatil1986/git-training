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
                if (employeeId != Guid.Empty && (entity.Stage ==3 || entity.Stage == 7))
                {
                    var description = String.Empty;

                    switch (entity.Stage)
                    {
                        case 3:
                            description = string.Format("Contact builder to coordinate a warranty introduction. Job # {0}", entity.JobNumber);
                            break;
                        case 7:
                            description = string.Format("Contact builder to coordinate a 244 walk. Job # {0}", entity.JobNumber);
                            break;
                    }
                    var task = new Task
                    {
                        EmployeeId = employeeId,
                        Description = description,
                        ReferenceId = entity.JobId,
                        TaskType = TaskType.JobStageChanged
                    };

                    _database.Insert(task);
                }
            }
        }
    }
}
