namespace Warranty.Core.Features.Report.WSRLoadingReport
{
    using System.Collections.Generic;
    using System.Linq;
    using Enumerations;
    using Extensions;
    using NPoco;
    using Common.Security.User.Session;

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
                    EmployeeTiedToRepresentatives = GetEmployeesTiedToRepresentatives(),
                };

            if (model.EmployeeTiedToRepresentatives.Count() == 1)
            {
                model.SelectedEmployeeNumber = model.EmployeeTiedToRepresentatives.First().EmployeeNumber;
                model.LoadingSummaries = GetWSRLoadingSummary(model.EmployeeTiedToRepresentatives.First().EmployeeNumber).ToList();
            }
            else
            {
                model.LoadingSummaries = GetWSRLoadingSummary(query.queryModel != null ? query.queryModel.SelectedEmployeeNumber : "").ToList();
            }
            
            model.TotalWarrantableHomesUnderOneYear = model.LoadingSummaries.Sum(lines => lines.NumberOfWarrantableHomesUnderOneYear);
            model.TotalWarrantableHomesUnderTwoYear = model.LoadingSummaries.Sum(lines => lines.NumberOfWarrantableHomesUnderTwoYear);
            model.TotalNonWarrantableHomes = model.LoadingSummaries.Sum(lines => lines.NumberOfNonWarrantableHomes);
            model.AnyResults = model.LoadingSummaries.Any();

            return model;
        }

        private IEnumerable<WSRLoadingReportModel.LoadingSummary> GetWSRLoadingSummary(string employeeNumber)
        {
            var user = _userSession.GetCurrentUser();
            using (_database)
            {
                const string sql = @"; WITH AllWSRCommunities (JobId, CloseDate, CommunityId, EmployeeId)
                                        AS
                                        (
                                            SELECT j.JobId, j.CloseDate, j.CommunityId, e.EmployeeId
                                            FROM Jobs j
                                            INNER JOIN Communities c
                                            ON j.CommunityId = c.CommunityId
                                            INNER JOIN Cities Ci
                                            ON c.CityId = Ci.CityId
                                            INNER JOIN CommunityAssignments ca
                                            ON c.CommunityId = ca.CommunityId
                                            INNER JOIN Employees e
                                            ON ca.EmployeeId = e.EmployeeId
                                            WHERE CloseDate <= @0
                                            AND Ci.CityCode IN ({0})
                                            AND EmployeeNumber = @3
                                        )

                                        SELECT e.EmployeeName, e.EmployeeNumber, c.CommunityName, a.*
                                        FROM
                                        (
                                            SELECT CommunityId, EmployeeId,
                                                SUM(CASE WHEN CloseDate >= DATEADD(yy, @1, @0) THEN 1 ELSE 0 END) as NumberOfWarrantableHomesUnderOneYear,
                                                SUM(CASE WHEN CloseDate >= DATEADD(yy, @2, @0) THEN 1 ELSE 0 END) as NumberOfWarrantableHomesUnderTwoYear,
                                                SUM(CASE WHEN CloseDate < DATEADD(yy, @2, @0) THEN 1 ELSE 0 END) as NumberOfNonWarrantableHomes
                                            FROM AllWSRCommunities
                                            GROUP BY CommunityId, EmployeeId
                                        ) a
                                        INNER JOIN Communities c
                                        ON a.CommunityId = c.CommunityId
                                        INNER JOIN Employees e
                                        ON a.EmployeeId = e.EmployeeId
                                        ORDER BY c.CommunityName";

                var result = _database.Fetch<WSRLoadingReportModel.LoadingSummary>(string.Format(sql, user.Markets.CommaSeparateWrapWithSingleQuote()), SystemTime.Today, -1, -2, employeeNumber);

                return result;
            }
        }

        private IEnumerable<WSRLoadingReportModel.EmployeeTiedToRepresentative> GetEmployeesTiedToRepresentatives()
        {
            var user = _userSession.GetCurrentUser();

            const string sql = @"SELECT DISTINCT e.EmployeeId as WarrantyRepresentativeEmployeeId, e.EmployeeNumber, LOWER(e.EmployeeName) as EmployeeName from CommunityAssignments ca
                                    INNER join Communities c
                                    ON ca.CommunityId = c.CommunityId
                                    INNER join Employees e
                                    ON ca.EmployeeId = e.EmployeeId
                                    INNER JOIN Cities ci
                                    ON c.CityId = ci.CityId
                                    WHERE CityCode IN ({0})
                                    {1} /* Additional Where */
                                    ORDER BY EmployeeName";

            var additionalWhereClause = "";

            if (user.IsInRole(UserRoles.WarrantyServiceRepresentative))
            {
                additionalWhereClause += "AND EmployeeNumber = " + user.EmployeeNumber + "";
            }

            using (_database)
            {
                var result = _database.Fetch<WSRLoadingReportModel.EmployeeTiedToRepresentative>(string.Format(sql, user.Markets.CommaSeparateWrapWithSingleQuote(), additionalWhereClause));

                return result;
            }
        }
    }
}