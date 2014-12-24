using System;

namespace Warranty.Core.Services
{
    using NPoco;

    public class CommunityEmployeeService : ICommunityEmployeeService
    {
        private readonly IDatabase _database;

        public CommunityEmployeeService(IDatabase database)
        {
            _database = database;
        }

        public Guid? GetEmployeeIdForWsc(Guid jobId)
        {
            using (_database)
            {
                const string sqlEmployeeId = @"SELECT TOP 1 ca.EmployeeId
                                                    FROM CommunityAssignments ca
                                                    INNER JOIN Jobs j
                                                    ON ca.CommunityId = j.CommunityId
                                                    WHERE j.JobId = @0";


                var employeeId = _database.FirstOrDefault<Guid>(sqlEmployeeId, jobId);

                return employeeId == Guid.Empty ? (Guid?) null : employeeId;
            }
        }
    }
}
