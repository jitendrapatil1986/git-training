using System;

namespace Warranty.Server.Handlers.Jobs
{
    using Accounting.Events.Job;
    using Core.Entities;
    using Extensions;
    using NPoco;
    using NServiceBus;

    public class JobPrimaryBuilderUpdatedHandler : IHandleMessages<JobPrimaryBuilderUpdated>
    {
        private readonly IDatabase _database;

        public JobPrimaryBuilderUpdatedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(JobPrimaryBuilderUpdated message)
        {
            using (_database)
            {
                var builder = _database.SingleOrDefault<Employee>("WHERE EmployeeNumber = @0" ,message.PrimaryBuilderNumber);
                if (builder == null)
                {
                    builder = new Employee
                    {
                        Name = message.PrimaryBuilderName,
                        Number = message.PrimaryBuilderNumber,
                        CreatedBy = "JobPrimaryBuilderUpdatedHandler",
                        CreatedDate = DateTime.UtcNow
                    };
                                    
                    _database.Insert(builder);
                }

                var job = _database.SingleByJdeId<Job>(message.JDEId);
                job.BuilderEmployeeId = builder.EmployeeId;
                _database.Update(job);
            }
        }
    }
}
