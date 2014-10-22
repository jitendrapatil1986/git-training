namespace Warranty.Core.Features.Report.Achievement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Enumerations;
    using Extensions;
    using NPoco;
    using Security;
    using Survey.Client;
    using WarrantyBonusSummary;

    public class AchievementReportQueryHandler : IQueryHandler<AchievementReportQuery, AchievementReportModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;
        private readonly ISurveyClient _surveyClient;

        public AchievementReportQueryHandler(IDatabase database, IUserSession userSession, ISurveyClient surveyClient)
        {
            _database = database;
            _userSession = userSession;
            _surveyClient = surveyClient;
        }

        public AchievementReportModel Handle(AchievementReportQuery query)
        {
            var user = _userSession.GetCurrentUser();


            if (!query.queryModel.FilteredDate.HasValue)
            {
                return new AchievementReportModel
                {
                    EmployeeTiedToRepresentatives = GetEmployeesTiedToRepresentatives(),
                };
            }

            var model = new AchievementReportModel
                {
                    AchievementSummaries = GetAchievementSummary(query, user),
                    EmployeeTiedToRepresentatives = GetEmployeesTiedToRepresentatives(),
                };

            return model;
        }

        private IEnumerable<AchievementReportModel.AchievementSummary> GetAchievementSummary(AchievementReportQuery query, IUser user)
        {
            var surveyData = GetSurveyData(query, user);

            var start = query.queryModel.FilteredDate.Value.AddMonths(-12);
            var end = query.queryModel.FilteredDate.Value.ToLastDay();



            var reportRange = Enumerable.Range(1, 12)
                                 .Select(start.AddMonths)
                                 .TakeWhile(e => e <= end)
                                 .Select(e => new { Month = Convert.ToInt16(e.ToString("MM")), Year = Convert.ToInt16(e.ToString("yyyy")) }
                );


            var averageDays = GetAverageDaysClosed(query, user);
            var percentClosedWithin7Days = GetPercentClosedWithin7Days(query, user);
            var amountSpent = GetAmountSpent(query, user);
            var excellentService = GetEWS(surveyData);
            var definetelyWouldRecommend = GetDWR(surveyData);

            var list = new List<AchievementReportModel.AchievementSummary>();
            foreach (var range in reportRange)
            {
                var rangeAverageDays = averageDays.SingleOrDefault(x => x.MonthNumber == range.Month && x.YearNumber == range.Year);
                var rangePercentClosedWithin7Days = percentClosedWithin7Days.SingleOrDefault(x => x.MonthNumber == range.Month && x.YearNumber == range.Year);
                var rangeAmountSpent = amountSpent.SingleOrDefault(x => x.MonthNumber == range.Month && x.YearNumber == range.Year);
                var rangeExcellentService = excellentService.SingleOrDefault(x => x.MonthNumber == range.Month && x.YearNumber == range.Year);
                var rangeDefinetelyWouldRecommend = definetelyWouldRecommend.SingleOrDefault(x => x.MonthNumber == range.Month && x.YearNumber == range.Year);

                list.Add(new AchievementReportModel.AchievementSummary
                    {
                        AverageDaysClosing = GetAmountFromDto(rangeAverageDays),
                        PercentComplete7Days = GetAmountFromDto(rangePercentClosedWithin7Days),
                        AmountSpentPerHome = GetAmountFromDto(rangeAmountSpent),
                        EWS = GetAmountFromDto(rangeExcellentService),
                        DWR = GetAmountFromDto(rangeDefinetelyWouldRecommend),
                        Month = range.Month,
                        Year = range.Year
                    });
            }
            return list.OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);
        }

        private static decimal GetAmountFromDto(AchievementReportModel.AchievementDto dto)
        {
            return dto != null ? dto.Amount : 0;
        }


        private IEnumerable<AchievementReportModel.AchievementDto> GetAverageDaysClosed(AchievementReportQuery query, IUser user)
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
                                            WHERE CompletionDate >= dateadd(M,1, CONVERT(DATE,DATEADD(DD, -DAY(@0) + 1, DATEADD(YY, -1, @0))))
                                            AND CompletionDate <= DATEADD(M, 1, @0)
                                              AND CityCode IN ({0})
                                              AND EmployeeNumber=@1
									 group by month(completiondate), year(completionDate)";

            return GetAchievementSqlresult(query, user, sql);

        }

        private IEnumerable<AchievementReportModel.AchievementDto> GetPercentClosedWithin7Days(AchievementReportQuery query, IUser user)
        {

            const string sql = @"SELECT SUM(CASE WHEN DATEDIFF(DD, sc.CreatedDate, CompletionDate) <= 7 THEN 1 ELSE 0 END) * 100.0/COUNT(*) as Amount,  month(completiondate) MonthNumber, year(completionDate) YearNumber
								    FROM ServiceCalls sc
								    INNER JOIN Employees e
								    ON sc.WarrantyRepresentativeEmployeeId = e.EmployeeId
								    INNER JOIN Jobs j
								    ON sc.JobId = j.JobId
								    INNER JOIN Communities c
								    ON j.CommunityId = c.CommunityId
								    INNER JOIN Cities cc
								    ON c.CityId = cc.CityId
									WHERE CompletionDate >= dateadd(M,1, CONVERT(DATE,DATEADD(DD, -DAY(@0) + 1, DATEADD(YY, -1, @0))))
                                    AND CompletionDate <= DATEADD(M, 1, @0)
                                              AND CityCode IN ({0})
                                              AND EmployeeNumber=@1
									 group by month(completiondate), year(completionDate)";

            return GetAchievementSqlresult(query, user, sql);
        }

        private IEnumerable<AchievementReportModel.AchievementDto> GetAmountSpent(AchievementReportQuery query, IUser user)
        {
            var warrantablehomes = GetWarrantableHomes(query, user).ToList();
            var dollarsSpent = GetDollarSpent(query, user).ToList();

            for (var i = 0; i < dollarsSpent.Count(); i++)
            {
                yield return new AchievementReportModel.AchievementDto
                    {
                        Amount = dollarsSpent[i].Amount / warrantablehomes[i].Amount,
                        MonthNumber = dollarsSpent[i].MonthNumber,
                        YearNumber = dollarsSpent[i].YearNumber
                    };
            }
        }

        private IEnumerable<AchievementReportModel.AchievementDto> GetWarrantableHomes(AchievementReportQuery query, IUser user)
        {

            const string sql = @";with months as (select 0 as m union select 1 as m union select 2  union select 3 union select 4 union select 5 union select 6 union select 7 union select 8 union select 9 union select 10 union select 11)
                                        , dates as (SELECT CONVERT(DATE, DATEADD(DD, -DAY(DATEADD(MM, 1, @0)) + 1, DATEADD(MM, 1, @0))) AS FirstDayNextMonth, DATEADD(YY, -2, CONVERT(DATE, DATEADD(DD, -DAY(@0) + 1, @0))) AS FirstDayOfMonthTwoYearsAgo)
                                        , datesPerMonth as (select YEAR(DATEADD(MM, -m, @0)) AS DateYear
				                                            , MONTH(DATEADD(MM, -m, @0)) AS DateMonth
				                                            , DATEADD(MM, -m, FirstDayNextMonth) as FirstDayNextMonth
				                                            , DATEADD(MM, -m, FirstDayOfMonthTwoYearsAgo) as FirstDayOfMonthTwoYearsAgo 
				                                            FROM dates
				                                            CROSS JOIN months)
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
                                                                    AND EmployeeNumber=@1)
                                        SELECT COALESCE(COUNT(*), 0) as Amount, DateMonth MonthNumber, DateYear YearNumber
                                                                    FROM datesPerMonth dpm			
					                                           LEFT JOIN houses ON		   
						                                          CloseDate >= FirstDayOfMonthTwoYearsAgo 
						                                          AND CloseDate < FirstDayNextMonth
					                                           group by DateMonth, DateYear
                                                                order by DateYear, DateMonth";

            return GetAchievementSqlresult(query, user, sql);
        }

        private IEnumerable<AchievementReportModel.AchievementDto> GetDollarSpent(AchievementReportQuery query, IUser user)
        {

            const string sql = @";with months as (select 0 as m union select 1 as m union select 2  union select 3 union select 4 union select 5 union select 6 union select 7 union select 8 union select 9 union select 10 union select 11)
                                    , dates as (SELECT CONVERT(DATE, DATEADD(DD, -DAY(DATEADD(MM, 1, @0)) + 1, DATEADD(MM, 1, @0))) AS FirstDayNextMonth,CONVERT(DATE, DATEADD(DD, -DAY(@0) + 1, @0)) AS FirstDayOfMonth)
                                    , datesPerMonth as (select YEAR(DATEADD(MM, -m, @0)) AS DateYear
				                                        , MONTH(DATEADD(MM, -m, @0)) AS DateMonth
				                                        , DATEADD(MM, -m, FirstDayNextMonth) as FirstDayNextMonth
				                                        , DATEADD(MM, -m, FirstDayOfMonth) as FirstDayOfMonth
				                                        FROM dates
				                                        CROSS JOIN months)
                                    , dollarSpent as (SELECT DatePosted, Amount FROM WarrantyPayments p								
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
								                                        AND EmployeeNumber=@1)
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
                                                                AND EmployeeNumber=@1)
					                                       SELECT COALESCE(SUM(Amount), 0) as Amount, DateMonth MonthNumber, DateYear YearNumber
                                                                        FROM datesPerMonth pdm
							                                     LEFT JOIN dollarSpent ON 
								                                    DatePosted >= pdm.FirstDayOfMonth
								                                    AND DatePosted < pdm.FirstDayNextMonth 
							                                      group by DateMonth, DateYear
							                                      order by DateYear, DateMonth";

            return GetAchievementSqlresult(query, user, sql);
        }

        private IEnumerable<AchievementReportModel.AchievementDto> GetAchievementSqlresult(AchievementReportQuery query, IUser user, string sql)
        {
            var employeeNumber = GetEmployeeNumber(query, user);
            var userMarkets = user.Markets.CommaSeparateWrapWithSingleQuote();
            using (_database)
            {
                var result = _database.Fetch<AchievementReportModel.AchievementDto>(string.Format(sql, userMarkets), query.queryModel.FilteredDate, employeeNumber);
                return result;
            }
        }

        private static string GetEmployeeNumber(AchievementReportQuery query, IUser user)
        {
            var employeeNumber = user.IsInRole(UserRoles.WarrantyServiceRepresentative)
                                     ? user.EmployeeNumber
                                     : query.queryModel.SelectedEmployeeNumber;
            return employeeNumber;
        }

        private IEnumerable<AchievementReportModel.AchievementDto> GetEWS(IEnumerable<AchievementReportModel.SurveyDataResult> surveyData)
        {
            return
                surveyData.GroupBy(x => new { x.SurveyDate.Month, x.SurveyDate.Year })
                          .Select(l => new AchievementReportModel.AchievementDto
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

        private IEnumerable<AchievementReportModel.AchievementDto> GetDWR(IEnumerable<AchievementReportModel.SurveyDataResult> surveyData)
        {
            return
                surveyData.GroupBy(x => new { x.SurveyDate.Month, x.SurveyDate.Year })
                          .Select(l => new AchievementReportModel.AchievementDto
                          {
                              Amount = l.Count(x => x.DefinitelyWillRecommend == "Definitely Will") / l.Count() * 100,
                              MonthNumber = l.Key.Month,
                              YearNumber = l.Key.Year
                          });

        }

        private IEnumerable<AchievementReportModel.EmployeeTiedToRepresentative> GetEmployeesTiedToRepresentatives()
        {
            var user = _userSession.GetCurrentUser();

            const string sql = @"SELECT DISTINCT EmployeeNumber, WarrantyRepresentativeEmployeeId
                                        , LOWER(e.EmployeeName) as EmployeeName
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
                                    AND EmployeeNumber <> ''
                                    {1} /* Additional Where */
                                    ORDER BY EmployeeName";

            var additionalWhereClause = "";

            if (user.IsInRole(UserRoles.WarrantyServiceRepresentative))
            {
                additionalWhereClause += "AND EmployeeNumber = " + user.EmployeeNumber + "";
            }

            using (_database)
            {
                var result = _database.Fetch<AchievementReportModel.EmployeeTiedToRepresentative>(string.Format(sql, user.Markets.CommaSeparateWrapWithSingleQuote(), additionalWhereClause));

                return result;
            }
        }

        private IEnumerable<AchievementReportModel.SurveyDataResult> GetSurveyData(AchievementReportQuery query, IUser user)
        {
            var employeeNumber = GetEmployeeNumber(query, user);
            var startDate = query.queryModel.FilteredDate.Value.AddMonths(-12).ToFirstDay();
            var endDate = query.queryModel.FilteredDate.Value.ToLastDay();
            var surveyData = _surveyClient.Get.ElevenMonthWarrantySurvey(new { startDate, endDate, EmployeeId = employeeNumber });
            return surveyData.Details.ToObject<List<AchievementReportModel.SurveyDataResult>>();
        }
    }
}