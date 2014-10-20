namespace Warranty.Core.TaskInfrastructure.Tasks
{
    using System;
    using Entities;
    using Enumerations;
    using Interfaces;
    using NPoco;

    public class JobClosedContactHomeownerTask : ITask<Job>
    {
        private readonly IDatabase _database;

        public JobClosedContactHomeownerTask(IDatabase database)
        {
            _database = database;
        }

        public void Create(Job entity)
        {
            using (_database)
            {
                var employeeId = _database.ExecuteScalar<Guid>("Select EmployeeId from CommunityAssignments where CommunityId = @0", entity.CommunityId);
                if (employeeId != Guid.Empty)
                {
                    var task = new Task
                    {
                        EmployeeId = employeeId,
                        Description = string.Format("Contact homeowner to coordinate warranty orientation. Job # {0}", entity.JobNumber),
                        ReferenceId = entity.JobId,
                        TaskType = TaskType.JobStageChanged
                    };
                    _database.Insert(task);
                }
            }
        }
    }
}
