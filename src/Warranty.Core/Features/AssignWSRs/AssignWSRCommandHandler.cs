using NPoco;
using Warranty.Core.Entities;

namespace Warranty.Core.Features.AssignWSRs
{
    public class AssignWSRCommandHandler : ICommandHandler<AssignWSRCommand>
    {
        private readonly IDatabase _database;

        public AssignWSRCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(AssignWSRCommand cmd)
        {
            using (_database)
            {
                const string sql = @"SELECT * FROM CommunityAssignments WHERE CommunityId = @0";

                var communityAssignment =
                    _database.FirstOrDefault<CommunityAssignment>(sql, cmd.CommunityId);

                if (communityAssignment == null)
                {
                    var newCommunityAssignment = new CommunityAssignment
                        {
                            CommunityId = cmd.CommunityId,
                            EmployeeId = cmd.EmployeeId,
                        };

                    _database.Insert(newCommunityAssignment);
                }
                else
                {
                    communityAssignment.EmployeeId = cmd.EmployeeId;
                    _database.Update(communityAssignment);
                }
            }
        }
    }
}