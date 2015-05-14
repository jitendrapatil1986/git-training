namespace Warranty.Core.Features.Report.WSRSummary
{
    using Enumerations;
    using NPoco;
    using Common.Extensions;
    using Common.Security.User.Session;

    public class WSRSummaryQueryHandler : IQueryHandler<WSRSummaryQuery, WSRSummaryModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public WSRSummaryQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public WSRSummaryModel Handle(WSRSummaryQuery query)
        {
            var user = _userSession.GetCurrentUser();
            var model = new WSRSummaryModel();

            using (_database)
            {
                const string sql = @"SELECT wsr.EmployeeNumber, wsr.EmployeeName, summary.NumberOfOpenServiceCalls, summary.NumberOfWarrantableHomes, summary.NumberOfNonWarrantableHomes FROM
                                    (
                                        SELECT DISTINCT e.EmployeeId, e.EmployeeNumber, LOWER(e.EmployeeName) as EmployeeName 
                                        FROM CommunityAssignments ca
                                        INNER join Communities c
                                        ON ca.CommunityId = c.CommunityId
                                        INNER join Employees e
                                        ON ca.EmployeeId = e.EmployeeId
                                        INNER JOIN Cities ci
                                        ON c.CityId = ci.CityId
                                        WHERE CityCode IN ({0})
                                        ) wsr
                                        INNER JOIN
                                        (
                                            SELECT a.EmployeeId, a.NumberOfOpenServiceCalls, b.NumberOfWarrantableHomes, c.NumberOfNonWarrantableHomes
                                            FROM
                                            (
                                                --open service calls
                                                SELECT e.EmployeeId, COUNT(*) as NumberOfOpenServiceCalls from ServiceCalls sc
                                                INNER JOIN Employees e
                                                ON sc.WarrantyRepresentativeEmployeeId = e.EmployeeId
                                                WHERE sc.ServiceCallStatusId = @2
                                                GROUP BY e.EmployeeId
                                            ) a
                                            LEFT JOIN
                                            (
                                                --warrantable homes
                                                SELECT COUNT(*) as NumberOfWarrantableHomes, e.EmployeeId
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
                                                GROUP BY e.EmployeeId
                                            ) b
                                            ON a.EmployeeId = b.EmployeeId
                                            LEFT JOIN
                                            (
                                                --non-warrantable homes
                                                SELECT COUNT(*) as NumberOfNonWarrantableHomes, e.EmployeeId
                                                FROM Jobs j
                                                INNER JOIN Communities c
                                                ON j.CommunityId = c.CommunityId
                                                INNER JOIN Cities Ci
                                                ON c.CityId = Ci.CityId
                                                INNER JOIN CommunityAssignments ca
                                                ON c.CommunityId = ca.CommunityId
                                                INNER JOIN Employees e
                                                ON ca.EmployeeId = e.EmployeeId
                                                WHERE CloseDate < DATEADD(yy, @0, @1)
                                                AND Ci.CityCode IN ({0})
                                                GROUP BY e.EmployeeId
                                            ) c
                                            ON b.EmployeeId = c.EmployeeId
                                    ) summary
                                    ON wsr.EmployeeId = summary.EmployeeId
                                    ORDER BY EmployeeName";
                
                model.WSRSummaryLines = _database.Fetch<WSRSummaryModel.WSRSummaryLine>(string.Format(sql, user.Markets.CommaSeparateWrapWithSingleQuote()), -2, SystemTime.Today, ServiceCallStatus.Open.Value);
                model.WSRSummaryLines.ForEach(x => x.EmployeeName = x.EmployeeName.ToTitleCase());
                return model;
            }
        }
    }
}