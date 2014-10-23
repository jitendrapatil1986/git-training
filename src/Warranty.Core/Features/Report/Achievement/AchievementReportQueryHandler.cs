namespace Warranty.Core.Features.Report.Achievement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Calculator;
    using Enumerations;
    using Extensions;
    using NPoco;
    using Security;
    using Survey.Client;

    public class AchievementReportQueryHandler : IQueryHandler<AchievementReportQuery, AchievementReportModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;
        private readonly IWarrantyCalculator _warrantyCalculator;

        public AchievementReportQueryHandler(IDatabase database, IUserSession userSession, ISurveyClient surveyClient, IWarrantyCalculator warrantyCalculator )
        {
            _database = database;
            _userSession = userSession;
            _warrantyCalculator = warrantyCalculator;
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
            var endDate = query.queryModel.FilteredDate.Value;
            var startDate = endDate.AddYears(-1);
            var employeeNumber = GetEmployeeNumber(query, user);

            var monthRange = Enumerable.Range(1, 12).Select(startDate.AddMonths).TakeWhile(e => e <= endDate).Select(e => new MonthYearModel { MonthNumber = Convert.ToInt16(e.ToString("MM")), YearNumber = Convert.ToInt16(e.ToString("yyyy")) });

            var excellentService = _warrantyCalculator.GetExcellentWarrantyService(startDate, endDate, employeeNumber);
            var definetelyWouldRecommend = _warrantyCalculator.GetDefinetelyWouldRecommend(startDate, endDate, employeeNumber);
            var rightTheFirstTime = _warrantyCalculator.GetRightTheFirstTime(startDate, endDate, employeeNumber);
            var amountSpent = _warrantyCalculator.GetAmountSpent(startDate, endDate, employeeNumber);
            var averageDays = _warrantyCalculator.GetAverageDaysClosed(startDate, endDate, employeeNumber);
            var percentClosedWithin7Days = _warrantyCalculator.GetPercentClosedWithin7Days(startDate, endDate, employeeNumber);

            return AgregateDataForReport(averageDays, percentClosedWithin7Days, amountSpent, excellentService, definetelyWouldRecommend, rightTheFirstTime, monthRange);
        }

        private IEnumerable<AchievementReportModel.AchievementSummary> AgregateDataForReport(IEnumerable<CalculatorResult> averageDays, IEnumerable<CalculatorResult> percentClosedWithin7Days, IEnumerable<CalculatorResult> amountSpent, IEnumerable<CalculatorResult> excellentService, IEnumerable<CalculatorResult> definetelyWouldRecommend, IEnumerable<CalculatorResult> rightTheFirstTime, IEnumerable<MonthYearModel> monthRange)
        {
            var list = new List<AchievementReportModel.AchievementSummary>();
            
            foreach (var range in monthRange)
            {
                list.Add(new AchievementReportModel.AchievementSummary
                    {
                        AverageDaysClosing = GetValueForMonth(averageDays, range),
                        PercentComplete7Days = GetValueForMonth(percentClosedWithin7Days, range),
                        AmountSpentPerHome = GetValueForMonth(amountSpent, range),
                        EWS = GetValueForMonth(excellentService, range),
                        DWR = GetValueForMonth(definetelyWouldRecommend, range),
                        RTFT = GetValueForMonth(rightTheFirstTime, range),
                        Month = range.MonthNumber,
                        Year = range.YearNumber
                    });
            }
            return list.OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);
        }

        private decimal GetValueForMonth(IEnumerable<CalculatorResult> results, MonthYearModel range)
        {
            var result =  results.SingleOrDefault(x => x.MonthNumber == range.MonthNumber && x.YearNumber == range.YearNumber);
            return result != null ? result.Amount : 0;
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

        private static string GetEmployeeNumber(AchievementReportQuery query, IUser user)
        {
            var employeeNumber = user.IsInRole(UserRoles.WarrantyServiceRepresentative) ? user.EmployeeNumber : query.queryModel.SelectedEmployeeNumber;
            return employeeNumber;
        }
    }
}