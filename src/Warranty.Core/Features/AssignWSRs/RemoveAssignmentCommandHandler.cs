using System.Linq;
using NPoco;
using Warranty.Core.Entities;
using Warranty.Core.Extensions;
using Common.Security.Session;

namespace Warranty.Core.Features.AssignWSRs
{
    public class RemoveAssignmentCommandHandler : ICommandHandler<RemoveAssignmentCommand>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public RemoveAssignmentCommandHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public void Handle(RemoveAssignmentCommand cmd)
        {
            var user = _userSession.GetCurrentUser();

            using (_database)
            {
                var marketList = user.Markets.CommaSeparate();

                using (_database)
                {
                    var assignment =
                        _database.Fetch<CommunityAssignment>(@"SELECT CA.*
                                    FROM Communities C
                                    INNER JOIN Cities CT ON
                                        @0 LIKE '%' + CT.CityCode + '%'
                                        AND C.CityId = CT.CityId
                                    INNER JOIN CommunityAssignments CA ON
                                        C.CommunityId = CA.CommunityId
                                        AND CA.EmployeeAssignmentId = @1", marketList, cmd.AssignmentId).Single();

                    _database.Delete(assignment);
                }
            }
        }
    }
}