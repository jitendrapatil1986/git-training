namespace Warranty.Core.Features.MyTeam
{
    using System.Collections.Generic;
    using Enumerations;
    using NPoco;
    using Common.Security.User.Session;
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
                var result = new List<MyTeamModel>();

                if (user.IsInRole(UserRoles.WarrantyServiceCoordinator) || user.IsInRole(UserRoles.CustomerCareManager))
                {
                    const string sql = @"SELECT DISTINCT e.EmployeeId, LOWER(e.EmployeeName) as EmployeeName from CommunityAssignments ca
                                    INNER join Communities c
                                    ON ca.CommunityId = c.CommunityId
                                    INNER join Employees e
                                    ON ca.EmployeeId = e.EmployeeId
                                    INNER JOIN Cities ci
                                    ON c.CityId = ci.CityId
                                    WHERE CityCode IN ({0})
                                    ORDER BY EmployeeName";

                    result = _database.Fetch<MyTeamModel>(string.Format(sql, user.Markets.CommaSeparateWrapWithSingleQuote()));
                }

                return result;
            }
        }
    }
}