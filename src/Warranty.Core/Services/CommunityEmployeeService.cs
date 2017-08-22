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
                const string sqlEmployeeId = @"Select Top 1 ca.EmployeeId from CommunityAssignments ca
                                                INNER JOIN Jobs j On ca.CommunityId = j.CommunityId
                                                Where j.JobId = @0 AND IsNull (AssignmentDate, '01-01-1753')=(Select MAX(IsNull (AssignmentDate, '01-01-1753')) from CommunityAssignments m where m.CommunityId = ca.CommunityId)";

                var employeeId = _database.FirstOrDefault<Guid>(sqlEmployeeId, jobId);
                
                return employeeId == Guid.Empty ? (Guid?) null : employeeId;
            }
        }
    }
}
