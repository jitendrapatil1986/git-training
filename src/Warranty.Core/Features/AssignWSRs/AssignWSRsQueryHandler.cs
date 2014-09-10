using System.Linq;
using NPoco;
using Warranty.Core.Entities;
using Warranty.Core.Extensions;
using Warranty.Core.Features.AmountSpentWidget;
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
                    model.Communities.AddRange(
                        _database.Fetch<Community>(@"SELECT C.CommunityId, MIN(C.CommunityNumber + '-' + C.CommunityName) AS CommunityName
                                    FROM Communities C
                                    INNER JOIN Cities CT ON
                                        @0 LIKE '%' + CT.CityCode + '%'
                                        AND LEN(LTRIM(RTRIM(CityCode))) = 3
                                        AND LEN(LTRIM(RTRIM(C.CommunityNumber))) = 4
                                    GROUP BY C.CommunityId
                                    ORDER BY CommunityName", marketList)
                            .Select(com => new AssignWSRsModel.Community
                            {
                                Id = com.CommunityId,
                                Name = com.CommunityName,
                                Employees = _database.Fetch<AssignedEmployee>(@"SELECT E.EmployeeName as Name, CA.EmployeeAssignmentId
                                            FROM Employees E
                                            INNER JOIN CommunityAssignments CA ON
                                                E.EmployeeId = CA.EmployeeId
                                                AND CA.CommunityId = @0
                                            ORDER BY E.EmployeeName", com.CommunityId).ToList()
                            }));

                    model.Employees =
                        _database.Fetch<Employee>("WHERE EmployeeName IS NOT NULL ORDER BY EmployeeName");
                }

                return model;
            }
        }
    }
}
