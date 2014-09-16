using System;
using NPoco;
using Warranty.Core.Entities;

namespace Warranty.Core.Features.CommunityEmployeeAssignment
{
    public class CommunityEmployeAssignmentCommandHandler : ICommandHandler<CommunityEmployeAssignmentCommand,bool>
    {
        private readonly IDatabase _database;

        public CommunityEmployeAssignmentCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public bool Handle(CommunityEmployeAssignmentCommand message)
        {
             using (_database)
             {
                 var employeeId = _database.SingleOrDefault<Guid>("SELECT E.EmployeeId FROM Employees e WHERE EmployeeNumber = @0", message.Employeenumber);

                 if (employeeId != Guid.Empty)
                 {
                     var communityEmployeeAssignment = new CommunityAssignment
                         {
                             EmployeeId = employeeId,
                             CommunityId = message.CommunityId
                         };
                     _database.Insert(communityEmployeeAssignment);
                     return true;
                 }
                 return false;
             }
        }
    }
}