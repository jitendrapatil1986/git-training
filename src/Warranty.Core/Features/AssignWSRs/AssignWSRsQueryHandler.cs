using System.Linq;
using NPoco;
using Warranty.Core.Entities;
using Warranty.Core.Extensions;
using Warranty.Core.Security;

namespace Warranty.Core.Features.AssignWSRs
{
    public class AssignWSRsQueryHandler : IQueryHandler<AssignWSRsQuery, AssignWSRsModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public AssignWSRsQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public AssignWSRsModel Handle(AssignWSRsQuery query)
        {
            var user = _userSession.GetCurrentUser();

            using (_database)
            {
                var marketList = user.Markets.CommaSeparate();

                var model = new AssignWSRsModel();

                using (_database)
                {
                    const string sql = @"SELECT a.CommunityId as Id, a.CommunityName as Name, COALESCE(ca.EmployeeAssignmentId, null) as EmployeeAssignmentId, COALESCE(e.EmployeeName, null) as Name FROM
                                        (SELECT C.CommunityId, MIN(C.CommunityNumber + '-' + C.CommunityName) AS CommunityName
                                            FROM Communities C
                                            INNER JOIN Cities CT ON
                                            @0 LIKE '%' + CT.CityCode + '%'
                                            AND CT.CityId = C.CityId
                                            GROUP BY C.CommunityId
                                         ) a
                                         LEFT JOIN CommunityAssignments ca
                                         ON a.CommunityId = ca.CommunityId
                                         LEFT JOIN Employees e
                                         ON e.EmployeeId = ca.EmployeeId
                                         ORDER BY CommunityName";

                    var communities = _database.FetchOneToMany<AssignWSRsModel.Community, AssignedEmployee>(x => x.Id, sql, marketList);

                    model.Communities = communities.OrderBy(x => x.IsAssigned).ThenBy(x => x.Name).ToList();
                }

                return model;
            }
        }
    }
}
