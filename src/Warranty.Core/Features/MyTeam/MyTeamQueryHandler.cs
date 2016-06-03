namespace Warranty.Core.Features.MyTeam
{
    using System.Collections.Generic;
    using Enumerations;
    using NPoco;
    using Common.Security.Session;
    using Extensions;

    public class MyTeamQueryHandler : IQueryHandler<MyTeamQuery, IEnumerable<MyTeamModel>>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public MyTeamQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public IEnumerable<MyTeamModel> Handle(MyTeamQuery query)
        {
            using (_database)
            {
                var user = _userSession.GetCurrentUser();
                
                const string sql = @"SELECT DISTINCT e.EmployeeId, LOWER(e.EmployeeName) as EmployeeName from CommunityAssignments ca
                                INNER join Communities c ON ca.CommunityId = c.CommunityId
                                INNER join Employees e ON ca.EmployeeId = e.EmployeeId
                                INNER JOIN Cities ci ON c.CityId = ci.CityId
                                WHERE ci.CityCode IN (@0)
                                ORDER BY EmployeeName";

                var result = _database.Fetch<MyTeamModel>(sql, user.Markets);

                return result;
            }
        }
    }
}