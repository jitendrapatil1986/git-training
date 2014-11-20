using System;
using System.Collections.Generic;

namespace Warranty.Core.Calculator
{
    using System.Linq;
    using Configurations;
    using Extensions;
    using NPoco;
    using Security;
    using Survey.Client;

    public class WarrantyCalculator : IWarrantyCalculator
    {
        private readonly IDatabase _database;
        private readonly ISurveyClient _surveyClient;
        private readonly string _userMarkets;
        public WarrantyCalculator(IDatabase database, IUserSession userSession, ISurveyClient surveyClient)
        {
            _database = database;
            _surveyClient = surveyClient;
            _userMarkets = userSession.GetCurrentUser().Markets.CommaSeparateWrapWithSingleQuote();
        }

        public IEnumerable<CalculatorResult> GetEmployeeAverageDaysClosed(DateTime startDate, DateTime endDate, string employeeNumber)
        {
            using (_database)
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
                                            WHERE CompletionDate >= @0
                                            AND CompletionDate <= @1
                                                AND CityCode IN ({0})
                                                AND EmployeeNumber=@2
									    group by month(completiondate), year(completionDate)";

                var result = _database.Fetch<CalculatorResult>(string.Format(sql, _userMarkets), startDate, endDate, employeeNumber);
                return result;
            }
        }

        public IEnumerable<CalculatorResult> GetEmployeePercentClosedWithin7Days(DateTime startDate, DateTime endDate, string employeeNumber)
        {
            using (_database)
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
						        WHERE CompletionDate >= @0
                                        AND CompletionDate <= @1
                                                AND CityCode IN ({0})
                                                AND EmployeeNumber=@2
									    group by month(completiondate), year(completionDate)";

                var result = _database.Fetch<CalculatorResult>(string.Format(sql, _userMarkets), startDate, endDate, employeeNumber);
                return result;
            }
        }

        public IEnumerable<CalculatorResult> GetEmployeeAmountSpent(DateTime startDate, DateTime endDate, string employeeNumber)
        {
            var warrantablehomes = GetEmployeeWarrantableHomes(startDate, endDate, employeeNumber).ToList();
            var dollarsSpent = GetEmployeeDollarSpent(startDate, endDate, employeeNumber).ToList();
            var monthRange = GetMonthRange(startDate, endDate);

            var list = new List<CalculatorResult>();
            foreach (var month in monthRange)
            {
                var dollarSpentInMonth = dollarsSpent.SingleOrDefault(x => x.MonthNumber == month.MonthNumber && x.YearNumber == month.YearNumber);
                var warrantableHomesInMonth = warrantablehomes.SingleOrDefault(x => x.MonthNumber == month.MonthNumber && x.YearNumber == month.YearNumber);

                list.Add(new CalculatorResult
                {
                    Amount = CalculateAmountSpentPerMonth(dollarSpentInMonth, warrantableHomesInMonth),
                    MonthNumber = month.MonthNumber,
                    YearNumber = month.YearNumber
                });
            }
            return list;
        }

        private IEnumerable<CalculatorResult> GetEmployeeWarrantableHomes(DateTime startDate, DateTime endDate, string employeeNumber)
        {
            using (_database)
            {
                const string sql =
                @";with e as (select 1 as n union all select 1 union all select 1 union all select 1 union all select 1 union all select 1 union all select 1 union all select 1 union all select 1 union all select 1)
                            , e2 as (select 1 as n from e a, e b)
                            , n as (select top 1200 /* 100 year span */ row_number() over (order by (select null)) - 1 number from e2 a, e2 b)
                            , months as (select MONTH(CONVERT(DATE,DATEADD(MM, -number, DATEADD(DD, -DAY(@1) + 1, @1)))) AS DateMonth
                                            , YEAR(CONVERT(DATE,DATEADD(MM, -number, DATEADD(DD, -DAY(@1) + 1, @1)))) AS DateYear
                                            , CONVERT(DATE,DATEADD(YY, -2, DATEADD(MM, -number, DATEADD(DD, -DAY(@1) + 1, @1)))) AS FirstOfMonthTwoYearsAgo
                                            , CONVERT(DATE,DATEADD(MM, -number + 1, DATEADD(DD, -DAY(@1) + 1, @1))) AS NextMonth
                                            FROM n
                                            WHERE DATEADD(MM, -number + 1, DATEADD(DD, -DAY(@1) + 1, @1)) > CONVERT(DATE, DATEADD(MM, 1, DATEADD(DD, -DAY(@0) + 1, @0))))
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

                var result = _database.Fetch<CalculatorResult>(string.Format(sql, _userMarkets), startDate, endDate, employeeNumber);
                return result;
            }
        }

        private IEnumerable<CalculatorResult> GetEmployeeDollarSpent(DateTime startDate, DateTime endDate, string employeeNumber)
        {
            using (_database)
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
                                                                cc.CityCode IN ({0}) AND
								                                DatePosted >= @0
								                                AND DatePosted < @1
							                                    group by month(datePosted), year(dateposted)
							                                    order by year(dateposted), month(datePosted);";

                var result = _database.Fetch<CalculatorResult>(string.Format(sql, _userMarkets), startDate, endDate, employeeNumber);
                return result;
            }
        }

        public IEnumerable<CalculatorResult> GetEmployeeExcellentWarrantyService(DateTime startDate, DateTime endDate, string employeeNumber)
        {
            var surveyData = GetEmployeeSurveyData(startDate, endDate, employeeNumber);
            return
                surveyData.GroupBy(x => new { x.SurveyDate.Month, x.SurveyDate.Year })
                          .Select(l => new CalculatorResult
                          {
                              Amount = l.Count(x => x.ExcellentWarrantyService == "10" || x.ExcellentWarrantyService == "9") / l.Count() * 100,
                              MonthNumber = l.Key.Month,
                              YearNumber = l.Key.Year
                          });

        }

        public IEnumerable<CalculatorResult> GetEmployeeRightTheFirstTime(DateTime startDate, DateTime endDate, string employeeNumber)
        {
            var surveyData = GetEmployeeSurveyData(startDate, endDate, employeeNumber);
            return
                surveyData.GroupBy(x => new { x.SurveyDate.Month, x.SurveyDate.Year })
                          .Select(l => new CalculatorResult
                          {
                              Amount = l.Count(x => x.RightFirstTime == "10" || x.RightFirstTime == "9") / l.Count() * 100,
                              MonthNumber = l.Key.Month,
                              YearNumber = l.Key.Year
                          });

        }

        public IEnumerable<CalculatorResult> GetEmployeeDefinetelyWouldRecommend(DateTime startDate, DateTime endDate, string employeeNumber)
        {
            var surveyData = GetEmployeeSurveyData(startDate, endDate, employeeNumber);
            return
                surveyData.GroupBy(x => new { x.SurveyDate.Month, x.SurveyDate.Year })
                          .Select(l => new CalculatorResult
                          {
                              Amount = l.Count(x => x.DefinitelyWillRecommend != null && x.DefinitelyWillRecommend.ToUpper() == SurveyConstants.DefinitelyWillThreshold) / l.Count() * 100,
                              MonthNumber = l.Key.Month,
                              YearNumber = l.Key.Year
                          });

        }

        private IEnumerable<SurveyDataResult> GetEmployeeSurveyData(DateTime startDate, DateTime endDate, string employeeNumber)
        {
            var surveyData = _surveyClient.Get.ElevenMonthWarrantySurvey(new { startDate, endDate, EmployeeId = employeeNumber });
            return surveyData.Details.ToObject<List<SurveyDataResult>>();
        }







        public IEnumerable<CalculatorResult> GetDivisionAverageDaysClosed(DateTime startDate, DateTime endDate, string divisionName)
        {
            using (_database)
            {
                const string sql = @"SELECT AVG(DATEDIFF(DD, sc.CreatedDate, CompletionDate)) as Amount, month(completiondate) MonthNumber, year(completionDate) YearNumber
                                            FROM ServiceCalls sc
                                            INNER JOIN Employees e
                                            ON sc.WarrantyRepresentativeEmployeeId = e.EmployeeId
                                            INNER JOIN Jobs j
                                            ON sc.JobId = j.JobId
                                            INNER JOIN Communities c
                                            ON j.CommunityId = c.CommunityId
                                            INNER JOIN Divisions d
                                            ON c.DivisionId = d.DivisionId
                                            INNER JOIN Cities cc
                                            ON c.CityId = cc.CityId
                                            WHERE CompletionDate >= @0
                                            AND CompletionDate <= @1
                                                AND CityCode IN ({0})
                                                AND d.DivisionName=@2
									    group by month(completiondate), year(completionDate)";

                var result = _database.Fetch<CalculatorResult>(string.Format(sql, _userMarkets), startDate, endDate, divisionName);
                return result;
            }
        }

        public IEnumerable<CalculatorResult> GetDivisionPercentClosedWithin7Days(DateTime startDate, DateTime endDate, string divisionName)
        {
            using (_database)
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
                                INNER JOIN Divisions d
                                ON c.DivisionId = d.DivisionId
								INNER JOIN Cities cc
								ON c.CityId = cc.CityId
						        WHERE CompletionDate >= @0
                                        AND CompletionDate <= @1
                                                AND CityCode IN ({0})
                                                AND d.DivisionName=@2
									    group by month(completiondate), year(completionDate)";

                var result = _database.Fetch<CalculatorResult>(string.Format(sql, _userMarkets), startDate, endDate, divisionName);
                return result;
            }
        }

        public IEnumerable<CalculatorResult> GetDivisionAmountSpent(DateTime startDate, DateTime endDate, string divisionName)
        {
            var warrantablehomes = GetDivisionWarrantableHomes(startDate, endDate, divisionName).ToList();
            var dollarsSpent = GetDivisionDollarSpent(startDate, endDate, divisionName).ToList();
            var monthRange = GetMonthRange(startDate, endDate);

            var list = new List<CalculatorResult>();
            foreach (var month in monthRange)
            {
                var dollarSpentInMonth = dollarsSpent.SingleOrDefault(x => x.MonthNumber == month.MonthNumber && x.YearNumber == month.YearNumber);
                var warrantableHomesInMonth = warrantablehomes.SingleOrDefault(x => x.MonthNumber == month.MonthNumber && x.YearNumber == month.YearNumber);

                list.Add(new CalculatorResult
                {
                    Amount = CalculateAmountSpentPerMonth(dollarSpentInMonth, warrantableHomesInMonth),
                    MonthNumber = month.MonthNumber,
                    YearNumber = month.YearNumber
                });
            }
            return list;
        }

        private IEnumerable<CalculatorResult> GetDivisionWarrantableHomes(DateTime startDate, DateTime endDate, string divisionName)
        {
            using (_database)
            {
                const string sql =
                @";with e as (select 1 as n union all select 1 union all select 1 union all select 1 union all select 1 union all select 1 union all select 1 union all select 1 union all select 1 union all select 1)
                            , e2 as (select 1 as n from e a, e b)
                            , n as (select top 1200 /* 100 year span */ row_number() over (order by (select null)) - 1 number from e2 a, e2 b)
                            , months as (select MONTH(CONVERT(DATE,DATEADD(MM, -number, DATEADD(DD, -DAY(@1) + 1, @1)))) AS DateMonth
                                            , YEAR(CONVERT(DATE,DATEADD(MM, -number, DATEADD(DD, -DAY(@1) + 1, @1)))) AS DateYear
                                            , CONVERT(DATE,DATEADD(YY, -2, DATEADD(MM, -number, DATEADD(DD, -DAY(@1) + 1, @1)))) AS FirstOfMonthTwoYearsAgo
                                            , CONVERT(DATE,DATEADD(MM, -number + 1, DATEADD(DD, -DAY(@1) + 1, @1))) AS NextMonth
                                            FROM n
                                            WHERE DATEADD(MM, -number + 1, DATEADD(DD, -DAY(@1) + 1, @1)) > CONVERT(DATE, DATEADD(MM, 1, DATEADD(DD, -DAY(@0) + 1, @0))))
                            , houses as (SELECT j.CloseDate, J.JobNumber FROM Jobs j 						  
                                                                INNER JOIN Communities c
                                                                ON j.CommunityId = c.CommunityId
                                                                INNER JOIN Cities Ci
                                                                ON c.CityId = Ci.CityId
                                                                INNER JOIN CommunityAssignments ca
                                                                ON c.CommunityId = ca.CommunityId
                                                                INNER JOIN Divisions d
                                                                ON c.DivisionId = d.DivisionId
                                                                INNER JOIN Employees e
                                                                ON ca.EmployeeId = e.EmployeeId
                                                                WHERE Ci.CityCode IN ({0})
                                                                AND d.DivisionName=@2)
                                    SELECT COALESCE(COUNT(CloseDate), 0) as Amount, DateMonth MonthNumber, DateYear YearNumber
                                                                FROM months dpm			
					                                        LEFT JOIN houses ON		   
						                                        CloseDate >= FirstOfMonthTwoYearsAgo 
						                                        AND CloseDate < NextMonth
					                                        group by DateMonth, DateYear
                                                            order by DateYear, DateMonth;";

                var result = _database.Fetch<CalculatorResult>(string.Format(sql, _userMarkets), startDate, endDate, divisionName);
                return result;
            }
        }

        private IEnumerable<CalculatorResult> GetDivisionDollarSpent(DateTime startDate, DateTime endDate, string divisionName)
        {
            using (_database)
            {

                const string sql =
                    @"SELECT COALESCE(SUM(Amount), 0) as Amount, month(datePosted)MonthNumber, year(dateposted) YearNumber
                                                                    FROM WarrantyPayments p								
                                                                        INNER JOIN Jobs j
                                                                        ON p.JobNumber = j.JobNumber
                                                                        INNER JOIN Communities c
                                                                        ON j.CommunityId = c.CommunityId
                                                                        INNER JOIN Divisions d
                                                                        ON c.DivisionId = d.DivisionId
                                                                        INNER JOIN Cities cc
                                                                        ON c.CityId = cc.CityId
                                                                        INNER JOIN CommunityAssignments ca
                                                                        ON c.CommunityId = ca.CommunityId
                                                                        INNER JOIN Employees e
                                                                        ON ca.EmployeeId = e.EmployeeId                                    
								                                    AND d.DivisionName=@2
                                                                where 
                                                                cc.CityCode IN ({0}) AND
								                                DatePosted >= @0
								                                AND DatePosted < @1
							                                    group by month(datePosted), year(dateposted)
							                                    order by year(dateposted), month(datePosted);";

                var result = _database.Fetch<CalculatorResult>(string.Format(sql, _userMarkets), startDate, endDate, divisionName);
                return result;
            }
        }

        public IEnumerable<CalculatorResult> GetDivisionExcellentWarrantyService(DateTime startDate, DateTime endDate, string divisionName)
        {
            var surveyData = GetDivisionSurveyData(startDate, endDate, divisionName);
            return
                surveyData.GroupBy(x => new { x.SurveyDate.Month, x.SurveyDate.Year })
                          .Select(l => new CalculatorResult
                          {
                              Amount = l.Count(x => x.ExcellentWarrantyService == "10" || x.ExcellentWarrantyService == "9") / l.Count() * 100,
                              MonthNumber = l.Key.Month,
                              YearNumber = l.Key.Year
                          });

        }

        public IEnumerable<CalculatorResult> GetDivisionRightTheFirstTime(DateTime startDate, DateTime endDate, string divisionName)
        {
            var surveyData = GetDivisionSurveyData(startDate, endDate, divisionName);
            return
                surveyData.GroupBy(x => new { x.SurveyDate.Month, x.SurveyDate.Year })
                          .Select(l => new CalculatorResult
                          {
                              Amount = l.Count(x => x.RightFirstTime == "10" || x.RightFirstTime == "9") / l.Count() * 100,
                              MonthNumber = l.Key.Month,
                              YearNumber = l.Key.Year
                          });

        }

        public IEnumerable<CalculatorResult> GetDivisionDefinetelyWouldRecommend(DateTime startDate, DateTime endDate, string divisionName)
        {
            var surveyData = GetDivisionSurveyData(startDate, endDate, divisionName);
            return
                surveyData.GroupBy(x => new { x.SurveyDate.Month, x.SurveyDate.Year })
                          .Select(l => new CalculatorResult
                          {
                              Amount = l.Count(x => x.DefinitelyWillRecommend != null && x.DefinitelyWillRecommend.ToUpper() == SurveyConstants.DefinitelyWillThreshold) / l.Count() * 100,
                              MonthNumber = l.Key.Month,
                              YearNumber = l.Key.Year
                          });

        }







        public IEnumerable<CalculatorResult> GetProjectAverageDaysClosed(DateTime startDate, DateTime endDate, string divisionName)
        {
            using (_database)
            {
                const string sql = @"SELECT AVG(DATEDIFF(DD, sc.CreatedDate, CompletionDate)) as Amount, month(completiondate) MonthNumber, year(completionDate) YearNumber
                                            FROM ServiceCalls sc
                                            INNER JOIN Employees e
                                            ON sc.WarrantyRepresentativeEmployeeId = e.EmployeeId
                                            INNER JOIN Jobs j
                                            ON sc.JobId = j.JobId
                                            INNER JOIN Communities c
                                            ON j.CommunityId = c.CommunityId
                                            INNER JOIN Projects pr
                                            ON c.ProjectId = pr.ProjectId
                                            INNER JOIN Cities cc
                                            ON c.CityId = cc.CityId
                                            WHERE CompletionDate >= @0
                                            AND CompletionDate <= @1
                                                AND CityCode IN ({0})
                                                AND pr.ProjectName=@2
									    group by month(completiondate), year(completionDate)";

                var result = _database.Fetch<CalculatorResult>(string.Format(sql, _userMarkets), startDate, endDate, divisionName);
                return result;
            }
        }

        public IEnumerable<CalculatorResult> GetProjectPercentClosedWithin7Days(DateTime startDate, DateTime endDate, string divisionName)
        {
            using (_database)
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
                                INNER JOIN Projects pr
                                ON c.ProjectId = pr.ProjectId
								INNER JOIN Cities cc
								ON c.CityId = cc.CityId
						        WHERE CompletionDate >= @0
                                        AND CompletionDate <= @1
                                                AND CityCode IN ({0})
                                                AND pr.ProjectName=@2
									    group by month(completiondate), year(completionDate)";

                var result = _database.Fetch<CalculatorResult>(string.Format(sql, _userMarkets), startDate, endDate, divisionName);
                return result;
            }
        }

        public IEnumerable<CalculatorResult> GetProjectAmountSpent(DateTime startDate, DateTime endDate, string divisionName)
        {
            var warrantablehomes = GetProjectWarrantableHomes(startDate, endDate, divisionName).ToList();
            var dollarsSpent = GetProjectDollarSpent(startDate, endDate, divisionName).ToList();
            var monthRange = GetMonthRange(startDate, endDate);

            var list = new List<CalculatorResult>();
            foreach (var month in monthRange)
            {
                var dollarSpentInMonth = dollarsSpent.SingleOrDefault(x => x.MonthNumber == month.MonthNumber && x.YearNumber == month.YearNumber);
                var warrantableHomesInMonth = warrantablehomes.SingleOrDefault(x => x.MonthNumber == month.MonthNumber && x.YearNumber == month.YearNumber);

                list.Add(new CalculatorResult
                {
                    Amount = CalculateAmountSpentPerMonth(dollarSpentInMonth, warrantableHomesInMonth),
                    MonthNumber = month.MonthNumber,
                    YearNumber = month.YearNumber
                });
            }
            return list;
        }

        private IEnumerable<CalculatorResult> GetProjectWarrantableHomes(DateTime startDate, DateTime endDate, string divisionName)
        {
            using (_database)
            {
                const string sql =
                @";with e as (select 1 as n union all select 1 union all select 1 union all select 1 union all select 1 union all select 1 union all select 1 union all select 1 union all select 1 union all select 1)
                            , e2 as (select 1 as n from e a, e b)
                            , n as (select top 1200 /* 100 year span */ row_number() over (order by (select null)) - 1 number from e2 a, e2 b)
                            , months as (select MONTH(CONVERT(DATE,DATEADD(MM, -number, DATEADD(DD, -DAY(@1) + 1, @1)))) AS DateMonth
                                            , YEAR(CONVERT(DATE,DATEADD(MM, -number, DATEADD(DD, -DAY(@1) + 1, @1)))) AS DateYear
                                            , CONVERT(DATE,DATEADD(YY, -2, DATEADD(MM, -number, DATEADD(DD, -DAY(@1) + 1, @1)))) AS FirstOfMonthTwoYearsAgo
                                            , CONVERT(DATE,DATEADD(MM, -number + 1, DATEADD(DD, -DAY(@1) + 1, @1))) AS NextMonth
                                            FROM n
                                            WHERE DATEADD(MM, -number + 1, DATEADD(DD, -DAY(@1) + 1, @1)) > CONVERT(DATE, DATEADD(MM, 1, DATEADD(DD, -DAY(@0) + 1, @0))))
                            , houses as (SELECT j.CloseDate, J.JobNumber FROM Jobs j 						  
                                                                INNER JOIN Communities c
                                                                ON j.CommunityId = c.CommunityId
                                                                INNER JOIN Cities Ci
                                                                ON c.CityId = Ci.CityId
                                                                INNER JOIN CommunityAssignments ca
                                                                ON c.CommunityId = ca.CommunityId
                                                                INNER JOIN Projects pr
                                                                ON c.ProjectId = pr.ProjectId
                                                                INNER JOIN Employees e
                                                                ON ca.EmployeeId = e.EmployeeId
                                                                WHERE Ci.CityCode IN ({0})
                                                                AND pr.ProjectName=@2)
                                    SELECT COALESCE(COUNT(CloseDate), 0) as Amount, DateMonth MonthNumber, DateYear YearNumber
                                                                FROM months dpm			
					                                        LEFT JOIN houses ON		   
						                                        CloseDate >= FirstOfMonthTwoYearsAgo 
						                                        AND CloseDate < NextMonth
					                                        group by DateMonth, DateYear
                                                            order by DateYear, DateMonth;";

                var result = _database.Fetch<CalculatorResult>(string.Format(sql, _userMarkets), startDate, endDate, divisionName);
                return result;
            }
        }

        private IEnumerable<CalculatorResult> GetProjectDollarSpent(DateTime startDate, DateTime endDate, string divisionName)
        {
            using (_database)
            {

                const string sql =
                    @"SELECT COALESCE(SUM(Amount), 0) as Amount, month(datePosted)MonthNumber, year(dateposted) YearNumber
                                                                    FROM WarrantyPayments p								
                                                                        INNER JOIN Jobs j
                                                                        ON p.JobNumber = j.JobNumber
                                                                        INNER JOIN Communities c
                                                                        ON j.CommunityId = c.CommunityId
                                                                        INNER JOIN Projects pr
                                                                        ON c.ProjectId = pr.ProjectId
                                                                        INNER JOIN Cities cc
                                                                        ON c.CityId = cc.CityId
                                                                        INNER JOIN CommunityAssignments ca
                                                                        ON c.CommunityId = ca.CommunityId
                                                                        INNER JOIN Employees e
                                                                        ON ca.EmployeeId = e.EmployeeId                                    
								                                    AND pr.ProjectName=@2
                                                                where 
                                                                cc.CityCode IN ({0}) AND
								                                DatePosted >= @0
								                                AND DatePosted < @1
							                                    group by month(datePosted), year(dateposted)
							                                    order by year(dateposted), month(datePosted);";

                var result = _database.Fetch<CalculatorResult>(string.Format(sql, _userMarkets), startDate, endDate, divisionName);
                return result;
            }
        }

        public IEnumerable<CalculatorResult> GetProjectExcellentWarrantyService(DateTime startDate, DateTime endDate, string divisionName)
        {
            var surveyData = GetProjectSurveyData(startDate, endDate, divisionName);
            return
                surveyData.GroupBy(x => new { x.SurveyDate.Month, x.SurveyDate.Year })
                          .Select(l => new CalculatorResult
                          {
                              Amount = l.Count(x => x.ExcellentWarrantyService == "10" || x.ExcellentWarrantyService == "9") / l.Count() * 100,
                              MonthNumber = l.Key.Month,
                              YearNumber = l.Key.Year
                          });

        }

        public IEnumerable<CalculatorResult> GetProjectRightTheFirstTime(DateTime startDate, DateTime endDate, string divisionName)
        {
            var surveyData = GetProjectSurveyData(startDate, endDate, divisionName);
            return
                surveyData.GroupBy(x => new { x.SurveyDate.Month, x.SurveyDate.Year })
                          .Select(l => new CalculatorResult
                          {
                              Amount = l.Count(x => x.RightFirstTime == "10" || x.RightFirstTime == "9") / l.Count() * 100,
                              MonthNumber = l.Key.Month,
                              YearNumber = l.Key.Year
                          });

        }

        public IEnumerable<CalculatorResult> GetProjectDefinetelyWouldRecommend(DateTime startDate, DateTime endDate, string divisionName)
        {
            var surveyData = GetProjectSurveyData(startDate, endDate, divisionName);
            return
                surveyData.GroupBy(x => new { x.SurveyDate.Month, x.SurveyDate.Year })
                          .Select(l => new CalculatorResult
                          {
                              Amount = l.Count(x => x.DefinitelyWillRecommend != null && x.DefinitelyWillRecommend.ToUpper() == SurveyConstants.DefinitelyWillThreshold) / l.Count() * 100,
                              MonthNumber = l.Key.Month,
                              YearNumber = l.Key.Year
                          });

        }

















        private IEnumerable<SurveyDataResult> GetDivisionSurveyData(DateTime startDate, DateTime endDate, string divisionName)
        {
            var surveyData = _surveyClient.Get.ElevenMonthWarrantySurvey(new { startDate, endDate, EmployeeId = string.Empty });
            var surveyDataResult = (List<SurveyDataResult>)surveyData.Details.ToObject<List<SurveyDataResult>>();
            return surveyDataResult.Where(x => x.Division == divisionName);
        }

        private IEnumerable<SurveyDataResult> GetProjectSurveyData(DateTime startDate, DateTime endDate, string projectName)
        {
            var surveyData = _surveyClient.Get.ElevenMonthWarrantySurvey(new { startDate, endDate, EmployeeId = string.Empty });
            var surveyDataResult = (List<SurveyDataResult>)surveyData.Details.ToObject<List<SurveyDataResult>>();
            return surveyDataResult.Where(x => x.Project == projectName);
        }

        public IEnumerable<MonthYearModel> GetMonthRange(DateTime startDate, DateTime endDate)
        {
            var numberOfMonths = (endDate.Month - startDate.Month) + 12 * (endDate.Year - startDate.Year) + 1;
            return Enumerable.Range(0, numberOfMonths).Select(startDate.AddMonths).TakeWhile(e => e <= endDate).Select(e => new MonthYearModel { MonthNumber = e.Month, YearNumber = e.Year });
        }

        private decimal CalculateAmountSpentPerMonth(CalculatorResult dollarSpentInMonth, CalculatorResult warrantableHomesInMonth)
        {
            if (dollarSpentInMonth != null && warrantableHomesInMonth != null)
            {
                if (dollarSpentInMonth.Amount == 0 || warrantableHomesInMonth.Amount == 0)
                {
                    return 0;
                }

                return dollarSpentInMonth.Amount / warrantableHomesInMonth.Amount;
            }
            return 0;
        }
    }
}
