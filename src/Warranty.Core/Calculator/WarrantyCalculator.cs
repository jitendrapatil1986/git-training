using System;
using System.Collections.Generic;

namespace Warranty.Core.Calculator
{
    using System.Linq;
    using Extensions;
    using NPoco;
    using Security;
    using Survey.Client;

    public class WarrantyCalculator : IWarrantyCalculator
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;
        private readonly ISurveyClient _surveyClient;

        public WarrantyCalculator(IDatabase database, IUserSession userSession, ISurveyClient surveyClient)
        {
            _database = database;
            _userSession = userSession;
            _surveyClient = surveyClient;
        }

        public IEnumerable<CalculatorResult> GetAverageDaysClosed(DateTime startDate, DateTime endDate, string employeeNumber)
        {
            const string sql = @"SELECT AVG(DATEDIFF(DD, sc.CreatedDate, CompletionDate)) as Amount, month(completiondate) MonthNumber, year(completionDate) YearNumber
                                            FROM ServiceCalls sc
                                            INNER JOIN Employees e
                                            ON sc.WarrantyRepresentativeEmployeeId = e.EmployeeId
                                            INNER JOIN Jobs j
                                            ON sc.JobId = j.JobId
                                            INNER JOIN Communities c
                                            ON j.CommunityId = c.CommunityId
                                            INNER JOIN Cities cc
                                            ON c.CityId = cc.CityId
                                            WHERE CompletionDate >= dateadd(M,1, CONVERT(DATE,DATEADD(DD, -DAY(@0) + 1, @0)))
                                            AND CompletionDate <= @1
                                                AND CityCode IN ({0})
                                                AND EmployeeNumber=@2
									    group by month(completiondate), year(completionDate)";


            return Execute(startDate, endDate, employeeNumber, sql);
        }

        public IEnumerable<CalculatorResult> GetPercentClosedWithin7Days(DateTime startDate, DateTime endDate, string employeeNumber)
        {
            const string sql =
                @"SELECT SUM(CASE WHEN DATEDIFF(DD, sc.CreatedDate, CompletionDate) <= 7 THEN 1 ELSE 0 END) * 100.0/COUNT(*) as Amount,  month(completiondate) MonthNumber, year(completionDate) YearNumber
								FROM ServiceCalls sc
								INNER JOIN Employees e
								ON sc.WarrantyRepresentativeEmployeeId = e.EmployeeId
								INNER JOIN Jobs j
								ON sc.JobId = j.JobId
								INNER JOIN Communities c
								ON j.CommunityId = c.CommunityId
								INNER JOIN Cities cc
								ON c.CityId = cc.CityId
						        WHERE CompletionDate >= dateadd(M,1, CONVERT(DATE,DATEADD(DD, -DAY(@0) + 1, @0)))
                                        AND CompletionDate <= @1
                                                AND CityCode IN ({0})
                                                AND EmployeeNumber=@2
									    group by month(completiondate), year(completionDate)";

            return Execute(startDate, endDate, employeeNumber, sql);
        }

        public IEnumerable<CalculatorResult> GetAmountSpent(DateTime startDate, DateTime endDate, string employeeNumber)
        {
            var warrantablehomes = GetWarrantableHomes(startDate, endDate, employeeNumber).ToList();
            var dollarsSpent = GetDollarSpent(startDate, endDate, employeeNumber).ToList();

            var list = new List<CalculatorResult>();
            for (var i = 0; i < dollarsSpent.Count(); i++)
            {
                list.Add(new CalculatorResult
                    {
                        Amount = dollarsSpent[i].Amount/warrantablehomes[i].Amount,
                        MonthNumber = dollarsSpent[i].MonthNumber,
                        YearNumber = dollarsSpent[i].YearNumber
                    });
            }
            return list;
        }

        private IEnumerable<CalculatorResult> GetWarrantableHomes(DateTime startDate, DateTime endDate, string employeeNumber)
        {
            const string sql =
                @";with e as (select 1 as n union all select 1 union all select 1 union all select 1 union all select 1 union all select 1 union all select 1 union all select 1 union all select 1 union all select 1)
                            , e2 as (select 1 as n from e a, e b)
                            , n as (select top 1200 /* 100 year span */ row_number() over (order by (select null)) - 1 number from e2 a, e2 b)
                            , months as (select MONTH(CONVERT(DATE,DATEADD(MM, -number, DATEADD(DD, -DAY(@0) + 1, @0)))) AS DateMonth
                                            , YEAR(CONVERT(DATE,DATEADD(MM, -number, DATEADD(DD, -DAY(@0) + 1, @0)))) AS DateYear
                                            , CONVERT(DATE,DATEADD(YY, -2, DATEADD(MM, -number, DATEADD(DD, -DAY(@0) + 1, @0)))) AS FirstOfMonthTwoYearsAgo
                                            , CONVERT(DATE,DATEADD(MM, -number + 1, DATEADD(DD, -DAY(@0) + 1, @0))) AS NextMonth
                                            FROM n
                                            WHERE DATEADD(MM, -number + 1, DATEADD(DD, -DAY(@1) + 1, @1)) >= @0)
                            , houses as (SELECT j.CloseDate, J.JobNumber FROM Jobs j 						  
                                                                INNER JOIN Communities c
                                                                ON j.CommunityId = c.CommunityId
                                                                INNER JOIN Cities Ci
                                                                ON c.CityId = Ci.CityId
                                                                INNER JOIN CommunityAssignments ca
                                                                ON c.CommunityId = ca.CommunityId
                                                                INNER JOIN Employees e
                                                                ON ca.EmployeeId = e.EmployeeId
                                                                WHERE Ci.CityCode IN ({0})
                                                                AND EmployeeNumber=@2)
                                    SELECT COALESCE(COUNT(CloseDate), 0) as Amount, DateMonth MonthNumber, DateYear YearNumber
                                                                FROM months dpm			
					                                        LEFT JOIN houses ON		   
						                                        CloseDate >= FirstOfMonthTwoYearsAgo 
						                                        AND CloseDate < NextMonth
					                                        group by DateMonth, DateYear
                                                            order by DateYear, DateMonth;";

            return Execute(startDate, endDate, employeeNumber, sql);
        }

        private IEnumerable<CalculatorResult> GetDollarSpent(DateTime startDate, DateTime endDate, string employeeNumber)
        {
            const string sql =
                @"SELECT COALESCE(SUM(Amount), 0) as Amount, month(datePosted)MonthNumber, year(dateposted) YearNumber
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
								                                    AND EmployeeNumber=@2
                                                                where 
								                                DatePosted >= @0
								                                AND DatePosted < @1
							                                    group by month(datePosted), year(dateposted)
							                                    order by year(dateposted), month(datePosted);";

            return Execute(startDate, endDate, employeeNumber, sql);
        }

        private IEnumerable<CalculatorResult> Execute(DateTime startDate, DateTime endDate, string employeeNumber, string sql)
        {
            using (_database)
            {
                startDate = startDate.ToFirstDay();
                endDate = endDate.ToLastDay();

                var user = _userSession.GetCurrentUser();
                var userMarkets = user.Markets.CommaSeparateWrapWithSingleQuote();

                var result = _database.Fetch<CalculatorResult>(string.Format(sql, userMarkets), startDate, endDate,
                                                               employeeNumber);
                return result;
            }
        }

        public IEnumerable<CalculatorResult> GetExcellentWarrantyService(DateTime startDate, DateTime endDate, string employeeNumber)
        {
            var surveyData = GetSurveyData(startDate, endDate, employeeNumber);
            return
                surveyData.GroupBy(x => new { x.SurveyDate.Month, x.SurveyDate.Year })
                          .Select(l => new CalculatorResult
                          {
                              Amount = (l.Count(x => x.ExcellentWarrantyService == "10" || x.ExcellentWarrantyService == "9") / l.Count() * 100 +
                              l.Count(x => x.ExcellentWarrantyService == "8" || x.ExcellentWarrantyService == "7") / l.Count() * 100 +
                              l.Count(x => x.ExcellentWarrantyService == "6" || x.ExcellentWarrantyService == "5") / l.Count() * 100 +
                              l.Count(x => x.ExcellentWarrantyService == "4" || x.ExcellentWarrantyService == "3") / l.Count() * 100 +
                              l.Count(x => x.ExcellentWarrantyService == "2" || x.ExcellentWarrantyService == "1") / l.Count() * 100) / 5,
                              MonthNumber = l.Key.Month,
                              YearNumber = l.Key.Year
                          });

        }

        public IEnumerable<CalculatorResult> GetRightTheFirstTime(DateTime startDate, DateTime endDate, string employeeNumber)
        {
            var surveyData = GetSurveyData(startDate, endDate, employeeNumber);
            return
                surveyData.GroupBy(x => new { x.SurveyDate.Month, x.SurveyDate.Year })
                          .Select(l => new CalculatorResult
                          {
                              Amount = (l.Count(x => x.RightFirstTime == "10" || x.RightFirstTime == "9") / l.Count() * 100 +
                              l.Count(x => x.RightFirstTime == "8" || x.RightFirstTime == "7") / l.Count() * 100 +
                              l.Count(x => x.RightFirstTime == "6" || x.RightFirstTime == "5") / l.Count() * 100 +
                              l.Count(x => x.RightFirstTime == "4" || x.RightFirstTime == "3") / l.Count() * 100 +
                              l.Count(x => x.RightFirstTime == "2" || x.RightFirstTime == "1") / l.Count() * 100) / 5,
                              MonthNumber = l.Key.Month,
                              YearNumber = l.Key.Year
                          });

        }

        public IEnumerable<CalculatorResult> GetDefinetelyWouldRecommend(DateTime startDate, DateTime endDate, string employeeNumber)
        {
            var surveyData = GetSurveyData(startDate, endDate, employeeNumber);
            return
                surveyData.GroupBy(x => new { x.SurveyDate.Month, x.SurveyDate.Year })
                          .Select(l => new CalculatorResult
                          {
                              Amount = l.Count(x => x.DefinitelyWillRecommend == "Definitely Will") / l.Count() * 100,
                              MonthNumber = l.Key.Month,
                              YearNumber = l.Key.Year
                          });

        }

        private IEnumerable<SurveyDataResult> GetSurveyData(DateTime startDate, DateTime endDate, string employeeNumber)
        {
            var surveyData = _surveyClient.Get.ElevenMonthWarrantySurvey(new { startDate, endDate, EmployeeId = employeeNumber });
            return surveyData.Details.ToObject<List<SurveyDataResult>>();
        }
    }
}
