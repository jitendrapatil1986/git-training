namespace Warranty.Core.Features.Report.WSRLoadingReport
{
    using System.Collections.Generic;
    using System.Linq;
    using Enumerations;
    using Extensions;
    using NPoco;
    using Security;

    public class WSRLoadingReportQueryHandler : IQueryHandler<WSRLoadingReportQuery, WSRLoadingReportModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public WSRLoadingReportQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public WSRLoadingReportModel Handle(WSRLoadingReportQuery query)
        {
            var model = new WSRLoadingReportModel
                {
                    LoadingSummaries = GetWSRLoadingSummary(query),
                    EmployeeTiedToRepresentatives = GetEmployeesTiedToRepresentatives(),
                };

            model.TotalNumberOfWarrantableHomes = model.LoadingSummaries.Sum(lines => lines.NumberOfWarrantableHomes);
            model.AnyResults = model.LoadingSummaries.Any();

            return model;
        }

        private IEnumerable<WSRLoadingReportModel.LoadingSummary> GetWSRLoadingSummary(WSRLoadingReportQuery query)
        {
            var user = _userSession.GetCurrentUser();

            var employeeNumber = "";

            if (query.queryModel != null)
            {
                employeeNumber = query.queryModel.SelectedEmployeeNumber;
            }

            using (_database)
            {
                const string sql = @"SELECT e.EmployeeName, e.EmployeeNumber, c.CommunityName, a.*
                                    FROM
                                    (
                                        SELECT COUNT(*) as NumberOfWarrantableHomes, j.CommunityId, e.EmployeeId
                                        FROM Jobs j
                                        INNER JOIN Communities c
                                        ON j.CommunityId = c.CommunityId
                                        INNER JOIN Cities Ci
                                        ON c.CityId = Ci.CityId
                                        INNER JOIN CommunityAssignments ca
                                        ON c.CommunityId = ca.CommunityId
                                        INNER JOIN Employees e
                                        ON ca.EmployeeId = e.EmployeeId
                                        WHERE CloseDate >= DATEADD(yy, @0, @1)
                                        AND CloseDate <= @1
                                        AND Ci.CityCode IN ({0})
                                        AND EmployeeNumber=@2
                                        GROUP BY j.CommunityId, e.EmployeeId
                                    ) a
                                    INNER JOIN Communities c
                                    ON a.CommunityId = c.CommunityId
                                    INNER JOIN Employees e
                                    ON a.EmployeeId = e.EmployeeId";

                var result = _database.Fetch<WSRLoadingReportModel.LoadingSummary>(string.Format(sql, user.Markets.CommaSeparateWrapWithSingleQuote()), -2, SystemTime.Today, employeeNumber);

                return result;
            }
        }

        private IEnumerable<WSRLoadingReportModel.EmployeeTiedToRepresentative> GetEmployeesTiedToRepresentatives()
        {
            var user = _userSession.GetCurrentUser();

            const string sql = @"SELECT e.EmployeeNumber, a.* FROM Employees e
                                INNER JOIN
                                (
                                    SELECT WarrantyRepresentativeEmployeeId
                                        , LOWER(e.EmployeeName) as EmployeeName
                                    FROM [ServiceCalls] sc
                                    INNER JOIN Employees e
                                    ON sc.WarrantyRepresentativeEmployeeId = e.EmployeeId
                                    INNER JOIN Jobs j
                                    ON sc.JobId = j.JobId
                                    INNER JOIN Communities cm
                                    ON j.CommunityId = cm.CommunityId
                                    INNER JOIN Cities ci
                                    ON cm.CityId = ci.CityId
                                    WHERE CityCode IN ({0})
                                    GROUP BY WarrantyRepresentativeEmployeeId, e.EmployeeName
                                ) a
                                ON e.EmployeeId = a.WarrantyRepresentativeEmployeeId
                                {1} /*WHERE */
                                ORDER BY e.EmployeeName";

            var whereClause = "WHERE EmployeeNumber <> ''";

            //if ((user.IsInRole(UserRoles.WarrantyServiceRepresentative) || user.IsInRole(UserRoles.WarrantyServiceManager)))
            if (user.IsInRole(UserRoles.WarrantyServiceRepresentative))
            {
                whereClause = "AND EmployeeNumber = " + user.EmployeeNumber + "";
            }

            var result = _database.Fetch<WSRLoadingReportModel.EmployeeTiedToRepresentative>(string.Format(sql, user.Markets.CommaSeparateWrapWithSingleQuote(), whereClause));

            return result;
        }
    }
}