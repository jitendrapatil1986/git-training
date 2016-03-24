using System;
using log4net;

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
        private readonly ILog _log;

        public JobPrimaryBuilderUpdatedHandler(IDatabase database, ILog log)
        {
            _database = database;
            _log = log;
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

                var job = _database.SingleOrDefaultByJdeId<Job>(message.JDEId);
                if (job == null)
                {
                    _log.WarnFormat("Could not update primary builder on job because job {0} does not exist locally", message.JDEId);
                    return;
                }

                job.BuilderEmployeeId = builder.EmployeeId;
                _database.Update(job);
            }
        }
    }
}
