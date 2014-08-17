using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
                        SeriesData = GetMyServiceTeamData(user),
                    };
            }
        }

        const string SqlTemplate = @"SELECT 
                                        WarrantyRepresentativeEmployeeId
                                        , e.EmployeeName
                                        , MONTH(sc.createddate) as [Month]
                                        , LEFT(DATENAME(m, sc.CreatedDate), 3) as [MonthName]
                                        , COUNT(*) as TotalCalls
                                        , SUM(CASE WHEN CompletionDate IS NULL THEN 1 ELSE 0 END) as [TotalOpen]
                                        , SUM(CASE WHEN CompletionDate IS NOT NULL THEN 1 ELSE 0 END) as [TotalClosed]
                                        , SUM(CASE WHEN CompletionDate IS NULL AND DATEADD(dd, 7, sc.CreatedDate) < getdate() THEN 1 ELSE 0 END) as [TotalOverdue]
                                    FROM [ServiceCalls] sc
                                    INNER JOIN Employees e
                                    ON sc.WarrantyRepresentativeEmployeeId = e.EmployeeId
                                    WHERE YEAR(sc.CreatedDate) = YEAR(getdate())
                                    GROUP BY WarrantyRepresentativeEmployeeId, e.EmployeeName, MONTH(sc.createddate), LEFT(DATENAME(m, sc.CreatedDate), 3)
                                    {0} /* WHERE */
                                    {1} /* ORDER BY */";


        const string SqlTemplateMonths = @"SELECT 
                                                DISTINCT MONTH(sc.createddate) as [Month]
                                                , LEFT(DATENAME(m, sc.CreatedDate), 3) as [MonthName]
                                            FROM [ServiceCalls] sc
                                            WHERE YEAR(sc.CreatedDate) = YEAR(getdate())
                                            GROUP BY MONTH(sc.createddate), LEFT(DATENAME(m, sc.CreatedDate), 3)
                                            {0} /* WHERE */
                                            {1} /* ORDER BY */";

        private string GetMyServiceTeamData(IUser user)
        {
            var sql = String.Format(SqlTemplate, "", "ORDER BY e.EmployeeName, [Month]");

            var result = _database.Fetch<MyServiceTeamWidgetModel.MyTeamChart>(sql);

            var myTeamChart = result.OrderBy(x => x.EmployeeName)
                                    .Select(x => String.Format("{{name:'{0}',data:[{1}]}}", x.EmployeeName, x.TotalOpen))
                                    .Distinct()
                                    .CommaSeparate();
            return myTeamChart;
        }

        private string GetMyServiceTeamDataMonths(IUser user)
        {
            var sql = String.Format(SqlTemplateMonths, "", "ORDER BY [Month]");

            var result = _database.Fetch<MyServiceTeamWidgetModel.MyTeamMonth>(sql);

            var monthString = result.Select(x => "'" + x.MonthName + "'")
                                    .ToArray()
                                    .CommaSeparate();

            return monthString;
        }
    }
}
