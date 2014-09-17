using System.Linq;
using NPoco;
using Warranty.Core.Entities;
using Warranty.Core.Extensions;
using Warranty.Core.Security;

namespace Warranty.Core.Features.AssignWSRs
{
    public class AssignWSRCommandHandler : ICommandHandler<AssignWSRCommand>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public AssignWSRCommandHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public void Handle(AssignWSRCommand cmd)
        {
            var user = _userSession.GetCurrentUser();

            using (_database)
            {
                var marketList = user.Markets.CommaSeparate();

                using (_database)
                {
                    var community =
                        _database.Fetch<Community>(@"SELECT C.CommunityId, MIN(C.CommunityNumber + '-' + C.CommunityName) AS CommunityName
                                    FROM Communities C
                                    INNER JOIN Cities CT ON
                                        @0 LIKE '%' + CT.CityCode + '%'
                                        AND C.CityId = CT.CityId
                                        AND C.CommunityId = @1
                                    GROUP BY C.CommunityId
                                    ORDER BY CommunityName", marketList, cmd.CommunityId).Single();

                    _database.Save<CommunityAssignment>(new CommunityAssignment
                    {
                        CommunityId = community.CommunityId,
                        EmployeeId = cmd.EmployeeId,
                    });
                }
            }
        }
    }
}