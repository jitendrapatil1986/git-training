namespace Warranty.Core.Features.Report.Achievement
{
    using System.Collections.Generic;
    using System.Linq;
    using Calculator;
    using Extensions;
    using NPoco;
    using Security;

    public class AchievementReportQueryHandler : IQueryHandler<AchievementReportQuery, AchievementReportModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;
        private readonly IWarrantyCalculator _warrantyCalculator;

        public AchievementReportQueryHandler(IDatabase database, IUserSession userSession, IWarrantyCalculator warrantyCalculator )
        {
            _database = database;
            _userSession = userSession;
            _warrantyCalculator = warrantyCalculator;
        }

        public AchievementReportModel Handle(AchievementReportQuery query)
        {
            var model = new AchievementReportModel
            {
                EmployeeTiedToRepresentatives = GetEmployeesTiedToRepresentatives(),
            };

            if (!query.queryModel.HasSearchCriteria)
                return model;

            var monthlyAchievementSummary = GetMonthlyAchievementSummary(query);
            var periodAchievementSummary = GetPeriodAchievementSummary(monthlyAchievementSummary);
            
            model.MonthlyAchievementSummary = monthlyAchievementSummary;
            model.PeriodAchievementSummary = periodAchievementSummary;
            
            return model;
        }

        private AchievementReportModel.AchievementSummary GetPeriodAchievementSummary(IEnumerable<AchievementReportModel.AchievementSummary> achievementSummaryByTheMonth)
        {
            return new AchievementReportModel.AchievementSummary
                {
                    AmountSpentPerHome = achievementSummaryByTheMonth.Average(x => x.AmountSpentPerHome),
                    AverageDaysClosing = achievementSummaryByTheMonth.Average(x => x.AverageDaysClosing),
                    DefinetelyWouldRecommend = achievementSummaryByTheMonth.Average(x => x.DefinetelyWouldRecommend),
                    ExcellentWarrantyService = achievementSummaryByTheMonth.Average(x => x.ExcellentWarrantyService),
                    RightTheFirstTime = achievementSummaryByTheMonth.Average(x => x.RightTheFirstTime),
                    PercentComplete7Days = achievementSummaryByTheMonth.Average(x => x.PercentComplete7Days),
                };
        }

        private IEnumerable<AchievementReportModel.AchievementSummary> GetMonthlyAchievementSummary(AchievementReportQuery query)
        {
            var employeeNumber = query.queryModel.SelectedEmployeeNumber;
            var startDate = query.queryModel.StartDate.Value;
            var endDate = query.queryModel.EndDate.Value.ToLastDay();

            var monthRange = _warrantyCalculator.GetMonthRange(startDate, endDate);

            var excellentService = _warrantyCalculator.GetEmployeeExcellentWarrantyService(startDate, endDate, employeeNumber);
            var definetelyWouldRecommend = _warrantyCalculator.GetEmployeeDefinetelyWouldRecommend(startDate, endDate, employeeNumber);
            var rightTheFirstTime = _warrantyCalculator.GetEmployeeRightTheFirstTime(startDate, endDate, employeeNumber);
            var amountSpent = _warrantyCalculator.GetEmployeeAmountSpent(startDate, endDate, employeeNumber);
            var averageDays = _warrantyCalculator.GetEmployeeAverageDaysClosed(startDate, endDate, employeeNumber);
            var percentClosedWithin7Days = _warrantyCalculator.GetEmployeePercentClosedWithin7Days(startDate, endDate, employeeNumber);

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
                        ExcellentWarrantyService = GetValueForMonth(excellentService, range),
                        DefinetelyWouldRecommend = GetValueForMonth(definetelyWouldRecommend, range),
                        RightTheFirstTime = GetValueForMonth(rightTheFirstTime, range),
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
            using (_database)
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
                                        ORDER BY EmployeeName";

                var result = _database.Fetch<AchievementReportModel.EmployeeTiedToRepresentative>(string.Format(sql, user.Markets.CommaSeparateWrapWithSingleQuote()));
                return result;
            }
        }
    }
}