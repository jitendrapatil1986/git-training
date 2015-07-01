using System;
using System.Collections.Generic;

namespace Warranty.Core.Calculator
{
    using System.Linq;
    using Configurations;
    using NPoco;
    using Services;

    public class WarrantyCalculator : IWarrantyCalculator
    {
        private readonly IDatabase _database;
        private readonly ISurveyService _surveyService;
        private readonly IEmployeeService _employeeService;
        private readonly string _userMarkets;

        public WarrantyCalculator(IDatabase database,  ISurveyService surveyService, IEmployeeService employeeService)
        {
            _database = database;
            _surveyService = surveyService;
            _employeeService = employeeService;
            _userMarkets = employeeService.GetEmployeeMarkets();
        }

        public IEnumerable<CalculatorResult> GetEmployeeAverageDaysClosed(DateTime startDate, DateTime endDate, string employeeNumber)
        {
            using (_database)
            {
                const string sql = @"SELECT count(*) as TotalElements, AVG(DATEDIFF(DD, sc.CreatedDate, CompletionDate)) as Amount, month(completiondate) MonthNumber, year(completionDate) YearNumber
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
                                            AND sc.ServiceCallType = 'Warranty Service Request'
                                     GROUP BY month(completiondate), year(completionDate)";

                var result = _database.Fetch<CalculatorResult>(string.Format(sql, _userMarkets), startDate, endDate, employeeNumber);
                return result;
            }
        }

        public IEnumerable<CalculatorResult> GetEmployeePercentClosedWithin7Days(DateTime startDate, DateTime endDate, string employeeNumber)
        {
            using (_database)
            {
                const string sql =
                    @"SELECT count(*) as TotalElements, SUM(CASE WHEN DATEDIFF(DD, sc.CreatedDate, CompletionDate) <= 7 THEN 1 ELSE 0 END) * 100.0/COUNT(*) as Amount,  month(completiondate) MonthNumber, year(completionDate) YearNumber
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
                            AND sc.ServiceCallType = 'Warranty Service Request'
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
                    YearNumber = month.YearNumber,
                    TotalElements = warrantableHomesInMonth != null ? warrantableHomesInMonth.TotalElements : 0
                });
            }
            return list;
        }

        public IEnumerable<CalculatorResult> GetEmployeeWarrantableHomes(DateTime startDate, DateTime endDate, string employeeNumber)
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
                                            WHERE DATEADD(MM, -number + 2, DATEADD(DD, -DAY(@1) + 1, @1)) > CONVERT(DATE, DATEADD(MM, 1, DATEADD(DD, -DAY(@0) + 1, @0))))
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
                                    
                            SELECT COALESCE(COUNT(CloseDate), 0) as TotalElements, DateMonth MonthNumber, DateYear YearNumber
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
                    @"; WITH AllWarrantyPayments (FiscalYear, Month, Year, CostCenter, ObjectAccount, Amount, CommunityNumber, FirstDayOfMonth, LastDayOfMonth)
                        AS
                        (
                            SELECT *, 
                                SUBSTRING(CostCenter, 1, 4) as CommunityNumber,
                                CONVERT(DATE, CONVERT(VARCHAR, Month) + '-1-' + CONVERT(VARCHAR, Year)) as FirstDayOfMonth ,
                                DATEADD(DD, -1, DATEADD(MM, 1, CONVERT(DATE, CONVERT(VARCHAR, Month) + '-1-' + CONVERT(VARCHAR, Year)))) as LastDayOfMonth 
                            FROM
                            (
                                SELECT GBFY as FiscalYear,
                                    CASE WHEN MonthAbbr = 'JAN' THEN 1
                                        WHEN MonthAbbr = 'FEB' THEN 2
                                        WHEN MonthAbbr = 'MAR' THEN 3
                                        WHEN MonthAbbr = 'APR' THEN 4
                                        WHEN MonthAbbr = 'MAY' THEN 5
                                        WHEN MonthAbbr = 'JUN' THEN 6
                                        WHEN MonthAbbr = 'JUL' THEN 7
                                        WHEN MonthAbbr = 'AUG' THEN 8
                                        WHEN MonthAbbr = 'SEP' THEN 9
                                        WHEN MonthAbbr = 'OCT' THEN 10
                                        WHEN MonthAbbr = 'NOV' THEN 11
                                        WHEN MonthAbbr = 'DEC' THEN 12
                                        ELSE 0
                                    END as Month,
                                    CASE WHEN LEN(GBFY) = 1 THEN RIGHT('200' + CAST(GBFY as VARCHAR(4)), 4)
                                        WHEN LEN(GBFY) = 2 THEN RIGHT('20' + CAST(GBFY as VARCHAR(4)), 4)
                                        ELSE 0
                                    END as Year,
                                    LTRIM(GBMCU) as CostCenter,
                                    GBOBJ as ObjectAccount,
                                    Amount
                                FROM tmp_JDE_GL_War_Buckets
                                UNPIVOT (Amount
                                FOR MonthAbbr IN (JAN, FEB, MAR, APR, MAY, JUN, JUL, AUG, SEP, OCT, NOV, [DEC]))
                                AS UNPVTTable
                            ) a
                        )

                        SELECT COALESCE(SUM(Amount), 0) as Amount, Month as MonthNumber, Year as YearNumber
                        FROM 
                        (
                        SELECT DISTINCT p.* FROM AllWarrantyPayments p
                        INNER JOIN Communities c
                            ON p.CommunityNumber = c.CommunityNumber
                        INNER JOIN Jobs j
                            ON c.CommunityId = j.CommunityId
                        INNER JOIN Cities cc
                            ON c.CityId = cc.CityId
                        INNER JOIN CommunityAssignments ca
                            ON c.CommunityId = ca.CommunityId
                        INNER JOIN Employees e
                            ON ca.EmployeeId = e.EmployeeId
                            AND EmployeeNumber=@2
                        WHERE
                            cc.CityCode IN ({0})
                            AND p.FirstDayOfMonth >= @0
                            AND p.LastDayOfMonth <= @1
                            AND p.LastDayOfMonth <= DATEADD(yy, 2, j.CloseDate)
                        ) a
                        GROUP BY Month, Year";

                var result = _database.Fetch<CalculatorResult>(string.Format(sql, _userMarkets), startDate, endDate, employeeNumber);
                return result;
            }
        }

        public IEnumerable<CalculatorResult> GetEmployeeOutstandingWarrantyService(DateTime startDate, DateTime endDate, string employeeNumber)
        {
            var surveyData = GetEmployeeSurveyData(startDate, endDate, employeeNumber);
            return GetOutstandingWarrantyResults(surveyData);
        }

        public IEnumerable<CalculatorResult> GetEmployeeRightTheFirstTime(DateTime startDate, DateTime endDate, string employeeNumber)
        {
            var surveyData = GetEmployeeSurveyData(startDate, endDate, employeeNumber);
            return
                surveyData.GroupBy(x => new { x.SurveyDate.Month, x.SurveyDate.Year })
                          .Select(l => new CalculatorResult
                          {
                              Amount = Decimal.Divide(l.Count(x => x.RightFirstTime == "10" || x.RightFirstTime == "9"), l.Count()) * 100,
                              MonthNumber = l.Key.Month,
                              YearNumber = l.Key.Year,
                              TotalElements = l.Count()
                          });

        }

        public IEnumerable<CalculatorResult> GetEmployeeDefinetelyWouldRecommend(DateTime startDate, DateTime endDate, string employeeNumber)
        {
            var surveyData = GetEmployeeSurveyData(startDate, endDate, employeeNumber);
            return
                surveyData.GroupBy(x => new { x.SurveyDate.Month, x.SurveyDate.Year })
                          .Select(l => new CalculatorResult
                          {
                              Amount = Decimal.Divide(l.Count(x => x.DefinitelyWillRecommend != null && x.DefinitelyWillRecommend.ToUpper() == SurveyConstants.DefinitelyWillThreshold), l.Count()) * 100,
                              MonthNumber = l.Key.Month,
                              YearNumber = l.Key.Year,
                              TotalElements = l.Count()
                          });

        }

        private IEnumerable<SurveyDataResult> GetEmployeeSurveyData(DateTime startDate, DateTime endDate, string employeeNumber)
        {
            var surveyData = _surveyService.Execute(x => x.Get.ElevenMonthWarrantySurvey(new {startDate, endDate, EmployeeId = employeeNumber}));
            if(surveyData != null)
                return surveyData.Details.ToObject<List<SurveyDataResult>>();

            return new List<SurveyDataResult>();
        }

        public IEnumerable<CalculatorResult> GetDivisionAverageDaysClosed(DateTime startDate, DateTime endDate, string divisionName)
        {
            using (_database)
            {
                const string sql = @"SELECT count(*) as TotalElements, AVG(DATEDIFF(DD, sc.CreatedDate, CompletionDate)) as Amount, month(completiondate) MonthNumber, year(completionDate) YearNumber
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
                    @"SELECT count(*) as TotalElements, SUM(CASE WHEN DATEDIFF(DD, sc.CreatedDate, CompletionDate) <= 7 THEN 1 ELSE 0 END) * 100.0/COUNT(*) as Amount,  month(completiondate) MonthNumber, year(completionDate) YearNumber
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
                    YearNumber = month.YearNumber,
                    TotalElements = warrantableHomesInMonth != null ? warrantableHomesInMonth.TotalElements : 0
                });
            }
            return list;
        }

        public IEnumerable<CalculatorResult> GetDivisionWarrantableHomes(DateTime startDate, DateTime endDate, string divisionName)
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
                                            WHERE DATEADD(MM, -number + 2, DATEADD(DD, -DAY(@1) + 1, @1)) > CONVERT(DATE, DATEADD(MM, 1, DATEADD(DD, -DAY(@0) + 1, @0))))
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

                            SELECT COALESCE(COUNT(CloseDate), 0) as TotalElements, DateMonth MonthNumber, DateYear YearNumber
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
                    @"; WITH AllWarrantyPayments (FiscalYear, Month, Year, CostCenter, ObjectAccount, Amount, CommunityNumber, FirstDayOfMonth, LastDayOfMonth)
                        AS
                        (
                            SELECT *, 
                                SUBSTRING(CostCenter, 1, 4) as CommunityNumber,
                                CONVERT(DATE, CONVERT(VARCHAR, Month) + '-1-' + CONVERT(VARCHAR, Year)) as FirstDayOfMonth ,
                                DATEADD(DD, -1, DATEADD(MM, 1, CONVERT(DATE, CONVERT(VARCHAR, Month) + '-1-' + CONVERT(VARCHAR, Year)))) as LastDayOfMonth 
                            FROM
                            (
                                SELECT GBFY as FiscalYear,
                                    CASE WHEN MonthAbbr = 'JAN' THEN 1
                                        WHEN MonthAbbr = 'FEB' THEN 2
                                        WHEN MonthAbbr = 'MAR' THEN 3
                                        WHEN MonthAbbr = 'APR' THEN 4
                                        WHEN MonthAbbr = 'MAY' THEN 5
                                        WHEN MonthAbbr = 'JUN' THEN 6
                                        WHEN MonthAbbr = 'JUL' THEN 7
                                        WHEN MonthAbbr = 'AUG' THEN 8
                                        WHEN MonthAbbr = 'SEP' THEN 9
                                        WHEN MonthAbbr = 'OCT' THEN 10
                                        WHEN MonthAbbr = 'NOV' THEN 11
                                        WHEN MonthAbbr = 'DEC' THEN 12
                                        ELSE 0
                                    END as Month,
                                    CASE WHEN LEN(GBFY) = 1 THEN RIGHT('200' + CAST(GBFY as VARCHAR(4)), 4)
                                        WHEN LEN(GBFY) = 2 THEN RIGHT('20' + CAST(GBFY as VARCHAR(4)), 4)
                                        ELSE 0
                                    END as Year,
                                    LTRIM(GBMCU) as CostCenter,
                                    GBOBJ as ObjectAccount,
                                    Amount
                                FROM tmp_JDE_GL_War_Buckets
                                UNPIVOT (Amount
                                FOR MonthAbbr IN (JAN, FEB, MAR, APR, MAY, JUN, JUL, AUG, SEP, OCT, NOV, [DEC]))
                                AS UNPVTTable
                            ) a
                        )

                        SELECT COALESCE(SUM(Amount), 0) as Amount, Month as MonthNumber, Year as YearNumber
                        FROM 
                        (
                        SELECT DISTINCT p.* FROM AllWarrantyPayments p
                        INNER JOIN Communities c
                            ON p.CommunityNumber = c.CommunityNumber
                        INNER JOIN Jobs j
                            ON c.CommunityId = j.CommunityId
                        INNER JOIN Divisions d
                            ON c.DivisionId = d.DivisionId
                        INNER JOIN Cities cc
                            ON c.CityId = cc.CityId
                        INNER JOIN CommunityAssignments ca
                            ON c.CommunityId = ca.CommunityId
                        INNER JOIN Employees e
                            ON ca.EmployeeId = e.EmployeeId
                            AND d.DivisionName=@2
                        WHERE
                            cc.CityCode IN ({0})
                            AND p.FirstDayOfMonth >= @0
                            AND p.LastDayOfMonth <= @1
                            AND p.LastDayOfMonth <= DATEADD(yy, 2, j.CloseDate)
                        ) a
                        GROUP BY Month, Year";

                var result = _database.Fetch<CalculatorResult>(string.Format(sql, _userMarkets), startDate, endDate, divisionName);
                return result;
            }
        }

        public IEnumerable<CalculatorResult> GetDivisionOutstandingWarrantyService(DateTime startDate, DateTime endDate, string divisionName)
        {
            var surveyData = GetDivisionSurveyData(startDate, endDate, divisionName);
            return GetOutstandingWarrantyResults(surveyData);
        }

        private IEnumerable<CalculatorResult> GetOutstandingWarrantyResults(IEnumerable<SurveyDataResult> surveyData)
        {
            Int16 score = 0;
            return
                surveyData.Where(x => !string.IsNullOrEmpty(x.WarrantyServiceScore) && Int16.TryParse(x.WarrantyServiceScore, out score))
                          .GroupBy(x => new { x.SurveyDate.Month, x.SurveyDate.Year })
                          .Select(l => new CalculatorResult
                          {
                              Amount = Decimal.Divide(l.Count(x => Convert.ToInt16(x.WarrantyServiceScore) >= SurveyConstants.OutstandingWarrantyThreshold), l.Count()) * 100,
                              MonthNumber = l.Key.Month,
                              YearNumber = l.Key.Year,
                              TotalElements = l.Count()
                          });
        }
        public IEnumerable<CalculatorResult> GetDivisionRightTheFirstTime(DateTime startDate, DateTime endDate, string divisionName)
        {
            var surveyData = GetDivisionSurveyData(startDate, endDate, divisionName);
            return
                surveyData.GroupBy(x => new { x.SurveyDate.Month, x.SurveyDate.Year })
                          .Select(l => new CalculatorResult
                          {
                              Amount = Decimal.Divide(l.Count(x => x.RightFirstTime == "10" || x.RightFirstTime == "9"), l.Count()) * 100,
                              MonthNumber = l.Key.Month,
                              YearNumber = l.Key.Year,
                              TotalElements = l.Count()
                          });

        }

        public IEnumerable<CalculatorResult> GetDivisionDefinetelyWouldRecommend(DateTime startDate, DateTime endDate, string divisionName)
        {
            var surveyData = GetDivisionSurveyData(startDate, endDate, divisionName);
            return
                surveyData.GroupBy(x => new { x.SurveyDate.Month, x.SurveyDate.Year })
                          .Select(l => new CalculatorResult
                          {
                              Amount = Decimal.Divide(l.Count(x => x.DefinitelyWillRecommend != null && x.DefinitelyWillRecommend.ToUpper() == SurveyConstants.DefinitelyWillThreshold) , l.Count()) * 100,
                              MonthNumber = l.Key.Month,
                              YearNumber = l.Key.Year,
                              TotalElements = l.Count()
                          });

        }

        public IEnumerable<CalculatorResult> GetProjectAverageDaysClosed(DateTime startDate, DateTime endDate, string projectName)
        {
            using (_database)
            {
                const string sql = @"SELECT count(*) as TotalElements, AVG(DATEDIFF(DD, sc.CreatedDate, CompletionDate)) as Amount, month(completiondate) MonthNumber, year(completionDate) YearNumber
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

                var result = _database.Fetch<CalculatorResult>(string.Format(sql, _userMarkets), startDate, endDate, projectName);
                return result;
            }
        }

        public IEnumerable<CalculatorResult> GetProjectPercentClosedWithin7Days(DateTime startDate, DateTime endDate, string projectName)
        {
            using (_database)
            {
                const string sql =
                    @"SELECT count(*) as TotalElements, SUM(CASE WHEN DATEDIFF(DD, sc.CreatedDate, CompletionDate) <= 7 THEN 1 ELSE 0 END) * 100.0/COUNT(*) as Amount,  month(completiondate) MonthNumber, year(completionDate) YearNumber
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

                var result = _database.Fetch<CalculatorResult>(string.Format(sql, _userMarkets), startDate, endDate, projectName);
                return result;
            }
        }

        public IEnumerable<CalculatorResult> GetProjectAmountSpent(DateTime startDate, DateTime endDate, string projectName)
        {
            var warrantablehomes = GetProjectWarrantableHomes(startDate, endDate, projectName).ToList();
            var dollarsSpent = GetProjectDollarSpent(startDate, endDate, projectName).ToList();
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
                    YearNumber = month.YearNumber,
                    TotalElements = warrantableHomesInMonth != null ? warrantableHomesInMonth.TotalElements : 0
                });
            }
            return list;
        }

        public IEnumerable<CalculatorResult> GetProjectWarrantableHomes(DateTime startDate, DateTime endDate, string projectName)
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
                                            WHERE DATEADD(MM, -number + 2, DATEADD(DD, -DAY(@1) + 1, @1)) > CONVERT(DATE, DATEADD(MM, 1, DATEADD(DD, -DAY(@0) + 1, @0))))
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

                            SELECT COALESCE(COUNT(CloseDate), 0) as TotalElements, DateMonth MonthNumber, DateYear YearNumber
                            FROM months dpm
                            LEFT JOIN houses ON
                                CloseDate >= FirstOfMonthTwoYearsAgo 
                                AND CloseDate < NextMonth
                            group by DateMonth, DateYear
                            order by DateYear, DateMonth;";

                var result = _database.Fetch<CalculatorResult>(string.Format(sql, _userMarkets), startDate, endDate, projectName);
                return result;
            }
        }

        private IEnumerable<CalculatorResult> GetProjectDollarSpent(DateTime startDate, DateTime endDate, string projectName)
        {
            using (_database)
            {
                const string sql =
                    @"; WITH AllWarrantyPayments (FiscalYear, Month, Year, CostCenter, ObjectAccount, Amount, CommunityNumber, FirstDayOfMonth, LastDayOfMonth)
                        AS
                        (
                            SELECT *, 
                                SUBSTRING(CostCenter, 1, 4) as CommunityNumber,
                                CONVERT(DATE, CONVERT(VARCHAR, Month) + '-1-' + CONVERT(VARCHAR, Year)) as FirstDayOfMonth ,
                                DATEADD(DD, -1, DATEADD(MM, 1, CONVERT(DATE, CONVERT(VARCHAR, Month) + '-1-' + CONVERT(VARCHAR, Year)))) as LastDayOfMonth 
                            FROM
                            (
                                SELECT GBFY as FiscalYear,
                                    CASE WHEN MonthAbbr = 'JAN' THEN 1
                                        WHEN MonthAbbr = 'FEB' THEN 2
                                        WHEN MonthAbbr = 'MAR' THEN 3
                                        WHEN MonthAbbr = 'APR' THEN 4
                                        WHEN MonthAbbr = 'MAY' THEN 5
                                        WHEN MonthAbbr = 'JUN' THEN 6
                                        WHEN MonthAbbr = 'JUL' THEN 7
                                        WHEN MonthAbbr = 'AUG' THEN 8
                                        WHEN MonthAbbr = 'SEP' THEN 9
                                        WHEN MonthAbbr = 'OCT' THEN 10
                                        WHEN MonthAbbr = 'NOV' THEN 11
                                        WHEN MonthAbbr = 'DEC' THEN 12
                                        ELSE 0
                                    END as Month,
                                    CASE WHEN LEN(GBFY) = 1 THEN RIGHT('200' + CAST(GBFY as VARCHAR(4)), 4)
                                        WHEN LEN(GBFY) = 2 THEN RIGHT('20' + CAST(GBFY as VARCHAR(4)), 4)
                                        ELSE 0
                                    END as Year,
                                    LTRIM(GBMCU) as CostCenter,
                                    GBOBJ as ObjectAccount,
                                    Amount
                                FROM tmp_JDE_GL_War_Buckets
                                UNPIVOT (Amount
                                FOR MonthAbbr IN (JAN, FEB, MAR, APR, MAY, JUN, JUL, AUG, SEP, OCT, NOV, [DEC]))
                                AS UNPVTTable
                            ) a
                        )

                        SELECT COALESCE(SUM(Amount), 0) as Amount, Month as MonthNumber, Year as YearNumber
                        FROM 
                        (
                        SELECT DISTINCT p.* FROM AllWarrantyPayments p
                        INNER JOIN Communities c
                            ON p.CommunityNumber = c.CommunityNumber
                        INNER JOIN Jobs j
                            ON c.CommunityId = j.CommunityId
                        INNER JOIN Projects pr
                            ON c.ProjectId = pr.ProjectId
                        INNER JOIN Cities cc
                            ON c.CityId = cc.CityId
                        INNER JOIN CommunityAssignments ca
                            ON c.CommunityId = ca.CommunityId
                        INNER JOIN Employees e
                            ON ca.EmployeeId = e.EmployeeId
                            AND pr.ProjectName=@2
                        WHERE
                            cc.CityCode IN ({0})
                            AND p.FirstDayOfMonth >= @0
                            AND p.LastDayOfMonth <= @1
                            AND p.LastDayOfMonth <= DATEADD(yy, 2, j.CloseDate)
                        ) a
                        GROUP BY Month, Year";

                var result = _database.Fetch<CalculatorResult>(string.Format(sql, _userMarkets), startDate, endDate, projectName);
                return result;
            }
        }

        public IEnumerable<CalculatorResult> GetProjectOutstandingWarrantyService(DateTime startDate, DateTime endDate, string projectName)
        {
            var surveyData = GetProjectSurveyData(startDate, endDate, projectName);
            return GetOutstandingWarrantyResults(surveyData);
        }

        public IEnumerable<CalculatorResult> GetProjectRightTheFirstTime(DateTime startDate, DateTime endDate, string projectName)
        {
            var surveyData = GetProjectSurveyData(startDate, endDate, projectName);
            return
                surveyData.GroupBy(x => new { x.SurveyDate.Month, x.SurveyDate.Year })
                          .Select(l => new CalculatorResult
                          {
                              Amount = Decimal.Divide(l.Count(x => x.RightFirstTime == "10" || x.RightFirstTime == "9") , l.Count()) * 100,
                              MonthNumber = l.Key.Month,
                              YearNumber = l.Key.Year,
                              TotalElements = l.Count()
                          });

        }

        public IEnumerable<CalculatorResult> GetProjectDefinetelyWouldRecommend(DateTime startDate, DateTime endDate, string projectName)
        {
            var surveyData = GetProjectSurveyData(startDate, endDate, projectName);
            return
                surveyData.GroupBy(x => new { x.SurveyDate.Month, x.SurveyDate.Year })
                          .Select(l => new CalculatorResult
                          {
                              Amount = Decimal.Divide(l.Count(x => x.DefinitelyWillRecommend != null && x.DefinitelyWillRecommend.ToUpper() == SurveyConstants.DefinitelyWillThreshold), l.Count()) * 100,
                              MonthNumber = l.Key.Month,
                              YearNumber = l.Key.Year,
                              TotalElements = l.Count()
                          });

        }

        private IEnumerable<SurveyDataResult> GetDivisionSurveyData(DateTime startDate, DateTime endDate, string divisionName)
        {
            var surveyData = _surveyService.Execute(x => x.Get.ElevenMonthWarrantySurvey(new
            {
                StartDate = startDate,
                EndDate = endDate,
                EmployeeIds = _employeeService.GetEmployeesInMarket(),
            }));

            if (surveyData != null)
            {
                var surveyDataResult = (List<SurveyDataResult>) surveyData.Details.ToObject<List<SurveyDataResult>>();
                return surveyDataResult.Where(x => x.Division == divisionName);
            }

            return new List<SurveyDataResult>();
        }

        private IEnumerable<SurveyDataResult> GetProjectSurveyData(DateTime startDate, DateTime endDate, string projectName)
        {
            var surveyData = _surveyService.Execute(x => x.Get.ElevenMonthWarrantySurvey(new
            {
                StartDate = startDate,
                EndDate = endDate,
                EmployeeIds = _employeeService.GetEmployeesInMarket(),
            }));

            if (surveyData != null)
            {
                var surveyDataResult = (List<SurveyDataResult>) surveyData.Details.ToObject<List<SurveyDataResult>>();
                return surveyDataResult.Where(x => x.Project == projectName);
            }

            return new List<SurveyDataResult>();
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
                if (dollarSpentInMonth.Amount == 0 || warrantableHomesInMonth.TotalElements == 0)
                {
                    return 0;
                }

                return dollarSpentInMonth.Amount.Value / warrantableHomesInMonth.TotalElements;
            }
            return 0;
        }
    }
}
