namespace Warranty.Core.Features.ServiceCallStats
{
    using NPoco;

    public class ServiceCallStatsQueryHandler : IQueryHandler<ServiceCallStatsQuery, ServiceCallStatsModel>
    {
        private readonly IDatabase _database;

        public ServiceCallStatsQueryHandler(IDatabase database)
        {
            _database = database;
        }

        public ServiceCallStatsModel Handle(ServiceCallStatsQuery query)
        {
            using (_database)
            {
                var sql = @"SELECT AverageDaysClosed, PercentClosedWithinSevenDays, TotalDollarsSpent, NumberOfWarrantableHomes, TotalDollarsSpent/NumberOfWarrantableHomes as DollarsSpentPerHome, a.EmployeeId, a.EmployeeName, a.EmployeeNumber, a.CityCode
                                FROM
                                (
                                    SELECT AVG(DATEDIFF(DD, sc.CreatedDate, CompletionDate)) as AverageDaysClosed
                                         , SUM(CASE WHEN DATEDIFF(DD, sc.CreatedDate, CompletionDate) <= 7 THEN 1 ELSE 0 END) * 100.0/COUNT(*) as PercentClosedWithinSevenDays     
                                         , EmployeeId, EmployeeName, EmployeeNumber, CityCode
                                    FROM ServiceCalls sc
                                        INNER JOIN Employees e
                                        ON sc.WarrantyRepresentativeEmployeeId = e.EmployeeId
                                        INNER JOIN Jobs j
                                        ON sc.JobId = j.JobId
                                        INNER JOIN Communities c
                                        ON j.CommunityId = c.CommunityId
                                        INNER JOIN Cities cc
                                        ON c.CityId = cc.CityId
                                    WHERE MONTH(CompletionDate) = MONTH(@0) AND YEAR(CompletionDate) = YEAR(@0)
                                    GROUP BY EmployeeId, EmployeeName, EmployeeNumber, CityCode
                                ) as a
                                INNER JOIN 
                                (
                                    SELECT SUM(Amount) as TotalDollarsSpent
                                         , e.EmployeeId, EmployeeName, EmployeeNumber, CityCode
                                    FROM WarrantyPayments p
                                        INNER JOIN Jobs j
                                        ON p.JobNumber = j.JobNumber
                                        INNER JOIN Communities c
                                        ON j.CommunityId = c.CommunityId
                                        INNER JOIN Cities cc
                                        ON c.CityId = cc.CityId
                                        INNER JOIN CommunityAssignments ca
                                        ON c.CommunityId = ca.CommunityId
                                        INNER JOIN Employees e
                                        ON ca.EmployeeId = e.EmployeeId
                                    WHERE PostingMonth = MONTH(@0) AND PostingYear = YEAR(@0)
                                    GROUP BY e.EmployeeId, EmployeeName, EmployeeNumber, CityCode
                                ) as b
                                ON a.EmployeeId = b.EmployeeId
                                INNER JOIN 
                                (
                                    SELECT COUNT(*) as NumberOfWarrantableHomes
                                         , e.EmployeeId, EmployeeName, EmployeeNumber, CityCode
                                    FROM Jobs j
                                        INNER JOIN Communities c
                                        ON j.CommunityId = c.CommunityId
                                        INNER JOIN Cities Ci
                                        ON c.CityId = Ci.CityId
                                        INNER JOIN CommunityAssignments ca
                                        ON c.CommunityId = ca.CommunityId
                                        INNER JOIN Employees e
                                        ON ca.EmployeeId = e.EmployeeId
                                    WHERE CloseDate >= DATEADD(yy, @1, @0) AND CloseDate <= @0
                                    GROUP BY e.EmployeeId, EmployeeName, EmployeeNumber, CityCode
                                ) as c
                                ON a.EmployeeId = c.EmployeeId";

                return new ServiceCallStatsModel
                           {
                               LineItems = _database.Fetch<ServiceCallStatsModel.LineItem>(sql, SystemTime.Today, -2),
                           };
            }
        }
    }
}
