using System;
using System.Collections.Generic;
using System.Linq;

namespace Warranty.Core.Features.MyServiceTeamWidget
{
    using Enumerations;
    using Extensions;
    using NPoco;
    using Common.Security.Session;
    using Common.Security.User;

    public class MyServiceTeamWidgetQueryHandler : IQueryHandler<MyServiceTeamWidgetQuery, MyServiceTeamWidgetModel>
    {
        private readonly IUserSession _userSession;
        private readonly IDatabase _database;

        public MyServiceTeamWidgetQueryHandler(IDatabase database, IUserSession userSession)
        {
            _database = database;
            _userSession = userSession;
        }

        public MyServiceTeamWidgetModel Handle(MyServiceTeamWidgetQuery query)
        {
            var user = _userSession.GetCurrentUser();

            using (_database)
            {
                return new MyServiceTeamWidgetModel
                    {
                        Months = GetMyServiceTeamDataMonths(),
                        SeriesDataOpen = GetMyServiceTeamOpenData(user),
                        SeriesDataClosed = GetMyServiceTeamClosedData(user),
                        MyTeamChartEmployeeSummaries = GetMyServiceTeamDataSummary(user),
                    };
            }
        }

        private IEnumerable<string> GetMyServiceTeamDataMonths()
        {
            var currentQuarter = Month.FromValue(DateTime.Today.Month).Quarter;
            return Month.GetAll().Where(x => x.Quarter == currentQuarter).Select(x => x.Abbreviation);
        }

        private IEnumerable<MyServiceTeamWidgetModel.MyTeamChartData> GetMyServiceTeamDataSummary(IUser user)
        {
            const string sql = @"SELECT WarrantyRepresentativeEmployeeId
                                            , LOWER(e.EmployeeName) as EmployeeName
                                            , COUNT(*) as TotalCalls
                                            , SUM(CASE WHEN CompletionDate IS NULL THEN 1 ELSE 0 END) as [TotalOpen]
                                            , SUM(CASE WHEN CompletionDate IS NOT NULL THEN 1 ELSE 0 END) as [TotalClosed]
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
                                        ORDER BY e.EmployeeName";

            var result = _database.Fetch<MyServiceTeamWidgetModel.MyTeamChartData>(string.Format(sql, user.Markets.CommaSeparateWrapWithSingleQuote()));

            return result;
        }

        private IEnumerable<MyServiceTeamWidgetModel.Series> GetMyServiceTeamOpenData(IUser user)
        {
            const string sql = @"SELECT COUNT(*) as TotalOpen, e.EmployeeName, MONTH(sc.CreatedDate) as Month
                            FROM ServiceCalls sc
                            INNER JOIN Employees e
                            ON sc.WarrantyRepresentativeEmployeeId = e.EmployeeId
                            INNER JOIN Jobs j
                            ON sc.JobId = j.JobId
                            INNER JOIN Communities c
                            ON j.CommunityId = c.CommunityId
                            INNER JOIN Cities cy
                            ON c.CityId = cy.CityId
                            WHERE YEAR(sc.CreatedDate) = YEAR(getdate()) AND CityCode IN ({0})
                            GROUP BY e.EmployeeName, MONTH(sc.CreatedDate)
                            ORDER BY e.EmployeeName, MONTH(sc.CreatedDate)";

            return GetTeamData(user, sql, data => data.TotalOpen);
        }

        private IEnumerable<MyServiceTeamWidgetModel.Series> GetMyServiceTeamClosedData(IUser user)
        {
            const string sql = @"SELECT COUNT(*) as TotalClosed, e.EmployeeName, MONTH(sc.CompletionDate) as Month
                            FROM ServiceCalls sc
                            INNER JOIN Employees e
                            ON sc.WarrantyRepresentativeEmployeeId = e.EmployeeId
                            INNER JOIN Jobs j
                            ON sc.JobId = j.JobId
                            INNER JOIN Communities c
                            ON j.CommunityId = c.CommunityId
                            INNER JOIN Cities cy
                            ON c.CityId = cy.CityId
                            WHERE ServiceCallStatusId = 3 AND CityCode IN ({0})
                            GROUP BY e.EmployeeName, MONTH(sc.CompletionDate)
                            ORDER BY e.EmployeeName, MONTH(sc.CompletionDate)";

            return GetTeamData(user, sql, data => data.TotalClosed);
        }

        private IEnumerable<MyServiceTeamWidgetModel.Series> GetTeamData(IUser user, string sql, Func<MyServiceTeamWidgetModel.MyTeamChartData, int> selector)
        {
            var markets = user.Markets;
            var result = _database.Fetch<MyServiceTeamWidgetModel.MyTeamChartData>(string.Format(sql, markets.CommaSeparateWrapWithSingleQuote()));
            var employees = result.Select(x => x.EmployeeName).Distinct();

            var resultWithMonths = from month in Month.GetAll().Where(x => GetMyServiceTeamDataMonths().Contains(x.Abbreviation))
                                   join r in result on month.Value equals r.Month into rm
                                   from resultmonth in rm.DefaultIfEmpty(null)
                                   select
                                       new MyServiceTeamWidgetModel.MyTeamChartData
                                           {
                                               EmployeeName = resultmonth != null ? resultmonth.EmployeeName : "",
                                               TotalOpen = resultmonth != null ? resultmonth.TotalOpen : 0,
                                               WarrantyRepresentativeEmployeeId = resultmonth != null ? resultmonth.WarrantyRepresentativeEmployeeId : Guid.Empty,
                                               Month = month.Value
                                           };

            var series = employees.Select(x => new MyServiceTeamWidgetModel.Series
                                                   {
                                                       Data = resultWithMonths.Where(data => data.EmployeeName == x || data.EmployeeName == "")
                                                                              .Select(selector)
                                                                              .ToList(),
                                                       Name = x
                                                   });

            return series;
        }
    }
}
