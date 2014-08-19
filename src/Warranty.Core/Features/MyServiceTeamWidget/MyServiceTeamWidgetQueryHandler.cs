using System;
using System.Collections.Generic;
using System.Linq;

namespace Warranty.Core.Features.MyServiceTeamWidget
{
    using Extensions;
    using NPoco;
    using Security;

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
                        Months = GetMyServiceTeamDataMonths(user),
                        SeriesDataOpen = GetMyServiceTeamOpenData(user),
                        SeriesDataClosed = GetMyServiceTeamClosedData(user),
                        SeriesDataOverdue = GetMyServiceTeamOverdueData(user),
                        MyTeamChartEmployeeSummaries = GetMyServiceTeamDataSummary(user),
                    };
            }
        }

        const string SqlTemplate = @";WITH Months AS
                                    (
                                        SELECT DISTINCT MONTH(sc.createddate) AS MonthNumber
                                        , LEFT(DATENAME(m, sc.CreatedDate), 3) AS MonthText
                                        FROM [ServiceCalls] sc
                                        WHERE YEAR(sc.CreatedDate) = YEAR(getdate())
                                        GROUP BY MONTH(sc.createddate), LEFT(DATENAME(m, sc.CreatedDate), 3)
                                    ), 
                                    EmployeeList AS
                                    (
	                                    SELECT DISTINCT WarrantyRepresentativeEmployeeId
	                                    , e.EmployeeName	
	                                    FROM [ServiceCalls] sc
	                                    INNER JOIN Employees e
	                                    ON sc.WarrantyRepresentativeEmployeeId = e.EmployeeId
                                        INNER JOIN Jobs j
                                        ON sc.JobId = j.JobId
                                        INNER JOIN Communities cm
                                        ON j.CommunityId = cm.CommunityId
                                        INNER JOIN Cities ci
                                        ON cm.CityId = ci.CityId
	                                    {0} /* WHERE */
                                        GROUP BY WarrantyRepresentativeEmployeeId, e.EmployeeName
                                        HAVING COUNT(*) > 0	
                                    ),
                                    EmployeeData AS
                                    (
	                                    SELECT WarrantyRepresentativeEmployeeId
	                                    , e.EmployeeName
	                                    , MONTH(sc.createddate) AS MonthNumber
	                                    , LEFT(DATENAME(m, sc.CreatedDate), 3) AS MonthText
	                                    , COUNT(*) AS TotalCalls
	                                    , SUM(CASE WHEN CompletionDate IS NULL THEN 1 ELSE 0 END) AS [TotalOpen]
	                                    , SUM(CASE WHEN CompletionDate IS NOT NULL THEN 1 ELSE 0 END) AS [TotalClosed]
	                                    , SUM(CASE WHEN CompletionDate IS NULL AND DATEADD(dd, 7, sc.CreatedDate) < getdate() THEN 1 ELSE 0 END) AS [TotalOverdue]
	                                    FROM [ServiceCalls] sc
	                                    INNER JOIN Employees e
	                                    ON sc.WarrantyRepresentativeEmployeeId = e.EmployeeId

                                        INNER JOIN Jobs j
                                        ON sc.JobId = j.JobId
                                        INNER JOIN Communities cm
                                        ON j.CommunityId = cm.CommunityId
                                        INNER JOIN Cities ci
                                        ON cm.CityId = ci.CityId
                                        {0} /* WHERE */
	                                    GROUP BY WarrantyRepresentativeEmployeeId, e.EmployeeName, MONTH(sc.createddate), LEFT(DATENAME(m, sc.CreatedDate), 3)
                                    )

                                    SELECT Emp.WarrantyRepresentativeEmployeeId, UPPER(Emp.EmployeeName) AS EmployeeName, M.MonthNumber, M.MonthText,
		                                    ISNULL(ED.TotalCalls, 0) AS TotalCalls, ISNULL(ED.TotalClosed, 0) AS TotalClosed, ISNULL(ED.TotalOpen, 0) AS TotalOpen, ISNULL(ED.TotalOverdue, 0) AS TotalOverdue
                                    FROM Months M
                                    CROSS JOIN EmployeeList Emp
                                    LEFT JOIN EmployeeData ED
                                    ON M.MonthNumber = ED.MonthNumber AND
	                                    Emp.WarrantyRepresentativeEmployeeId = ED.WarrantyRepresentativeEmployeeId
                                    {1} /* ORDER BY */";

        const string SqlTemplateMonths = @"SELECT 
                                                DISTINCT MONTH(sc.createddate) as [Month]
                                                , LEFT(DATENAME(m, sc.CreatedDate), 3) as [MonthName]
                                            FROM [ServiceCalls] sc
                                            WHERE YEAR(sc.CreatedDate) = YEAR(getdate())
                                            GROUP BY MONTH(sc.createddate), LEFT(DATENAME(m, sc.CreatedDate), 3)
                                            {0} /* ORDER BY */";

        const string SqlTemplateSummary = @"SELECT
                                            WarrantyRepresentativeEmployeeId
                                            , LOWER(e.EmployeeName) as EmployeeName
                                            , COUNT(*) as TotalCalls
                                            , SUM(CASE WHEN CompletionDate IS NULL THEN 1 ELSE 0 END) as [TotalOpen]
                                            , SUM(CASE WHEN CompletionDate IS NOT NULL THEN 1 ELSE 0 END) as [TotalClosed]
                                            , SUM(CASE WHEN CompletionDate IS NULL AND DATEADD(dd, 7, sc.CreatedDate) < getdate() THEN 1 ELSE 0 END) as [TotalOverdue]
                                        FROM [ServiceCalls] sc
                                        INNER JOIN Employees e
                                        ON sc.WarrantyRepresentativeEmployeeId = e.EmployeeId

                                        INNER JOIN Jobs j
                                        ON sc.JobId = j.JobId
                                        INNER JOIN Communities cm
                                        ON j.CommunityId = cm.CommunityId
                                        INNER JOIN Cities ci
                                        ON cm.CityId = ci.CityId
                                        {0} /* WHERE */
                                        GROUP BY WarrantyRepresentativeEmployeeId, e.EmployeeName
                                        {1} /* ORDER BY */";


        private string GetMyServiceTeamOpenData(IUser user)
        {
            var markets = user.Markets;

            var sql = String.Format(SqlTemplate, "WHERE YEAR(sc.CreatedDate) = YEAR(getdate()) AND CityCode IN (" + markets.CommaSeparateWrapWithSingleQuote() + ")", "ORDER BY Emp.EmployeeName, M.MonthNumber");

            var result = _database.Fetch<MyServiceTeamWidgetModel.MyTeamChartEmployeeDetail>(sql);

            var myOpenTeamChart = result.OrderBy(x => x.EmployeeName)
                                    .Select(x => String.Format("{{name:'{0}',data:[{1}]}}", 
                                                                    x.EmployeeName, result.Where(y => y.EmployeeName == x.EmployeeName)
                                                                                    .OrderBy(y => y.EmployeeName)
                                                                                    .Select(y => y.TotalOpen).CommaSeparate()))
                                    .Distinct()
                                    .CommaSeparate();
            return myOpenTeamChart;
        }

        private string GetMyServiceTeamClosedData(IUser user)
        {
            var markets = user.Markets;

            var sql = String.Format(SqlTemplate, "WHERE YEAR(sc.CreatedDate) = YEAR(getdate()) AND CityCode IN (" + markets.CommaSeparateWrapWithSingleQuote() + ")", "ORDER BY Emp.EmployeeName, M.MonthNumber");

            var result = _database.Fetch<MyServiceTeamWidgetModel.MyTeamChartEmployeeDetail>(sql);

            var myClosedTeamChart = result.OrderBy(x => x.EmployeeName)
                                    .Select(x => String.Format("{{name:'{0}',data:[{1}]}}",
                                                                    x.EmployeeName, result.Where(y => y.EmployeeName == x.EmployeeName)
                                                                                    .OrderBy(y => y.EmployeeName)
                                                                                    .Select(y => y.TotalClosed).CommaSeparate()))
                                    .Distinct()
                                    .CommaSeparate();
            return myClosedTeamChart;
        }

        private string GetMyServiceTeamOverdueData(IUser user)
        {
            var markets = user.Markets;

            var sql = String.Format(SqlTemplate, "WHERE YEAR(sc.CreatedDate) = YEAR(getdate()) AND CityCode IN (" + markets.CommaSeparateWrapWithSingleQuote() + ")", "ORDER BY Emp.EmployeeName, M.MonthNumber");

            var result = _database.Fetch<MyServiceTeamWidgetModel.MyTeamChartEmployeeDetail>(sql);

            var myOverdueTeamChart = result.OrderBy(x => x.EmployeeName)
                                    .Select(x => String.Format("{{name:'{0}',data:[{1}]}}",
                                                                    x.EmployeeName, result.Where(y => y.EmployeeName == x.EmployeeName)
                                                                                    .OrderBy(y => y.EmployeeName)
                                                                                    .Select(y => y.TotalOverdue).CommaSeparate()))
                                    .Distinct()
                                    .CommaSeparate();
            return myOverdueTeamChart;
        }

        private string GetMyServiceTeamDataMonths(IUser user)
        {
            var sql = String.Format(SqlTemplateMonths, "ORDER BY [Month]");

            var result = _database.Fetch<MyServiceTeamWidgetModel.MyTeamMonth>(sql);

            var monthString = result.Select(x => "'" + x.MonthName + "'")
                                    .ToArray()
                                    .CommaSeparate();

            return monthString;
        }

        private IEnumerable<MyServiceTeamWidgetModel.MyTeamChartEmployeeSummary> GetMyServiceTeamDataSummary(IUser user)
        {
            var markets = user.Markets;

            var sql = String.Format(SqlTemplateSummary, "WHERE YEAR(sc.CreatedDate) = YEAR(getdate()) AND CityCode IN (" + markets.CommaSeparateWrapWithSingleQuote() + ")", "ORDER BY e.EmployeeName");

            var result = _database.Fetch<MyServiceTeamWidgetModel.MyTeamChartEmployeeSummary>(sql);
            return result;
        }
    }
}
