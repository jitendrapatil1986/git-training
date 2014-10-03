namespace Warranty.Core.Features.ServiceCallStats
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Enumerations;
    using NPoco;
    using Security;
    using Extensions;

    public class ServiceCallStatsQueryHandler : IQueryHandler<ServiceCallStatsQuery, ServiceCallStatsModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;

        public ServiceCallStatsQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public ServiceCallStatsModel Handle(ServiceCallStatsQuery query)
        {
            var defaultView = query.ViewId ?? StatView.DollarsSpent;
            var currentQuarter = Month.FromValue(SystemTime.Today.Month).Quarter;
            var months = Month.GetAll().Where(x => x.Quarter == currentQuarter);

            using (_database)
            {
                var statList = new List<ServiceCallStatsModel.LineItem>();
                var currentStats = new List<ServiceCallStatsModel.LineItem>();
                foreach (var month in months)
                {
                    var stats = GetStats(defaultView, new DateTime(SystemTime.Today.Year, month.Value, 1));
                    if (month.Value == SystemTime.Today.Month)
                    {
                        currentStats = stats;
                    }

                    statList.AddRange(stats);
                }

                var dollarsSpentSeries = statList.Select(x => x.EmployeeNumber).Distinct().Select(s => new ServiceCallStatsModel.Series<decimal> {Data = statList.Where(y => y.EmployeeNumber == s).Select(x => x.DollarsSpentPerHome).ToList(), Name = statList.First(y => y.EmployeeNumber == s).EmployeeName}).OrderBy(x=>x.Name).ToList();
                var avgDaysClosedSeries = statList.Select(x => x.EmployeeNumber).Distinct().Select(s => new ServiceCallStatsModel.Series<int> { Data = statList.Where(y => y.EmployeeNumber == s).Select(x => x.AverageDaysClosed).ToList(), Name = statList.First(y => y.EmployeeNumber == s).EmployeeName }).OrderBy(x => x.Name).ToList();
                var percentClosedSeries = statList.Select(x => x.EmployeeNumber).Distinct().Select(s => new ServiceCallStatsModel.Series<decimal> { Data = statList.Where(y => y.EmployeeNumber == s).Select(x => x.PercentClosedWithinSevenDays).ToList(), Name = statList.First(y => y.EmployeeNumber == s).EmployeeName }).OrderBy(x => x.Name).ToList();

                return new ServiceCallStatsModel
                {
                    CurrentQuarter = currentQuarter,
                    View = defaultView,
                    Months = months.Select(x => x.Abbreviation).ToArray(),
                    DollarsSpentSeriesList = dollarsSpentSeries,
                    AverageDaysClosedSeriesList = avgDaysClosedSeries,
                    PercentClosedSeriesList = percentClosedSeries,
                    LineItems = currentStats,
                };
            }
        }

        public List<ServiceCallStatsModel.LineItem> GetStats(StatView defaultView, DateTime date)
        {
            var user = _userSession.GetCurrentUser();
            var isEmployeeSpecific = user.IsInRole(UserRoles.WarrantyServiceRepresentative);

            var sql = @"SELECT COALESCE(AverageDaysClosed, 0) as AverageDaysClosed, COALESCE(PercentClosedWithinSevenDays, 0) as PercentClosedWithinSevenDays, COALESCE(TotalDollarsSpent, 0) as TotalDollarsSpent, NumberOfWarrantableHomes, COALESCE(TotalDollarsSpent/NumberOfWarrantableHomes, 0) as DollarsSpentPerHome, a.EmployeeId, a.EmployeeName, a.EmployeeNumber, a.CityCode
                                FROM
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
                                ) as c
                                ON a.EmployeeId = c.EmployeeId
                                WHERE a.CityCode in ({0}) {3}
                                ORDER BY {1} {2}, EmployeeName";

            var additionalWhereClause = isEmployeeSpecific ? " AND a.EmployeeNumber = '" + user.EmployeeNumber + "'" : "";
            var completedSql = string.Format(sql, user.Markets.CommaSeparateWrapWithSingleQuote(), defaultView.OrderByColumnName, defaultView.SortOrder, additionalWhereClause);

            return _database.Fetch<ServiceCallStatsModel.LineItem>(completedSql, date, -2);
        }
    }
}
