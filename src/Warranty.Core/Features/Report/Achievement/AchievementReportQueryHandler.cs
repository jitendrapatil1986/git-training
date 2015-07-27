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

        public AchievementReportQueryHandler(IDatabase database, IUserSession userSession, IWarrantyCalculator warrantyCalculator)
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
            
            model.MonthlyAchievementSummary = monthlyAchievementSummary.AchievementSummaries;
            model.PeriodAchievementSummary = periodAchievementSummary;
            
            return model;
        }

        private AchievementReportModel.AchievementSummary GetPeriodAchievementSummary(AchievementResults achievementSummaryByTheMonth)
        {
            var def = (achievementSummaryByTheMonth.DefinitelyWouldRecommendResults.Sum(w => w.TotalCalculableElements) /
                      achievementSummaryByTheMonth.DefinitelyWouldRecommendResults.Sum(w => w.TotalElements)) * 100;

            var outs = (achievementSummaryByTheMonth.OutstandingService.Sum(w => w.TotalCalculableElements) /
                      achievementSummaryByTheMonth.OutstandingService.Sum(w => w.TotalElements)) * 100;
            
            var right = (achievementSummaryByTheMonth.RightTheFirstTime.Sum(w => w.TotalCalculableElements) /
                      achievementSummaryByTheMonth.RightTheFirstTime.Sum(w => w.TotalElements)) * 100;

            return new AchievementReportModel.AchievementSummary
                {
                    AmountSpentPerHome = achievementSummaryByTheMonth.AchievementSummaries.Average(x => x.AmountSpentPerHome),
                    AverageDaysClosing = achievementSummaryByTheMonth.AchievementSummaries.Average(x => x.AverageDaysClosing),
                    DefinitelyWouldRecommend = def,
                    OutstandingWarrantyService = outs,
                    RightTheFirstTime = right,
                    PercentComplete7Days = achievementSummaryByTheMonth.AchievementSummaries.Average(x => x.PercentComplete7Days),
                };
        }

        public class AchievementResults
        {
            public IEnumerable<CalculatorResult> DefinitelyWouldRecommendResults { get; set; }
            public IEnumerable<CalculatorResult> RightTheFirstTime { get; set; }
            public IEnumerable<CalculatorResult> OutstandingService { get; set; }
            public IEnumerable<CalculatorResult> AverageDays { get; set; }
            public IEnumerable<CalculatorResult> PercentClosedWithin7Days { get; set; }
            public IEnumerable<CalculatorResult> AmountSpent { get; set; }
            public IEnumerable<AchievementReportModel.AchievementSummary> AchievementSummaries { get; set; }

        }
        private AchievementResults GetMonthlyAchievementSummary(AchievementReportQuery query)
        {
            var achievementResults = new AchievementResults();

            var employeeNumber = query.queryModel.SelectedEmployeeNumber;
            var startDate = query.queryModel.StartDate.Value;
            var endDate = query.queryModel.EndDate.Value.ToLastDay();

            var monthRange = _warrantyCalculator.GetMonthRange(startDate, endDate);

            achievementResults.OutstandingService = _warrantyCalculator.GetEmployeeOutstandingWarrantyService(startDate, endDate, employeeNumber);
            achievementResults.DefinitelyWouldRecommendResults = _warrantyCalculator.GetEmployeeDefinitelyWouldRecommend(startDate, endDate, employeeNumber);
            achievementResults.RightTheFirstTime = _warrantyCalculator.GetEmployeeRightTheFirstTime(startDate, endDate, employeeNumber);
            achievementResults.AmountSpent = _warrantyCalculator.GetEmployeeAmountSpent(startDate, endDate, employeeNumber);
            achievementResults.AverageDays = _warrantyCalculator.GetEmployeeAverageDaysClosed(startDate, endDate, employeeNumber);
            achievementResults.PercentClosedWithin7Days = _warrantyCalculator.GetEmployeePercentClosedWithin7Days(startDate, endDate, employeeNumber);
            achievementResults.AchievementSummaries = AgregateDataForReport(achievementResults, monthRange);

            return achievementResults;
        }

        private IEnumerable<AchievementReportModel.AchievementSummary> AgregateDataForReport(AchievementResults achievementResults, IEnumerable<MonthYearModel> monthRange)
        {
            var list = new List<AchievementReportModel.AchievementSummary>();

            foreach (var range in monthRange)
            {
                list.Add(new AchievementReportModel.AchievementSummary
                    {
                        AverageDaysClosing = GetValueForMonth(achievementResults.AverageDays, range) ?? 0,
                        PercentComplete7Days = GetValueForMonth(achievementResults.PercentClosedWithin7Days, range) ?? 0,
                        AmountSpentPerHome = GetValueForMonth(achievementResults.AmountSpent, range) ?? 0,
                        OutstandingWarrantyService = GetValueForMonth(achievementResults.OutstandingService, range),
                        DefinitelyWouldRecommend = GetValueForMonth(achievementResults.DefinitelyWouldRecommendResults, range),
                        RightTheFirstTime = GetValueForMonth(achievementResults.RightTheFirstTime, range),
                        Month = range.MonthNumber,
                        Year = range.YearNumber
                    });
            }
            return list.OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);
        }

        private decimal? GetValueForMonth(IEnumerable<CalculatorResult> results, MonthYearModel range)
        {
            var result = results.SingleOrDefault(x => x.MonthNumber == range.MonthNumber && x.YearNumber == range.YearNumber);
            return result != null ? result.Amount.Value : (decimal?)null;
        }

        private IEnumerable<AchievementReportModel.EmployeeTiedToRepresentative> GetEmployeesTiedToRepresentatives()
        {
            using (_database)
            {
                var user = _userSession.GetCurrentUser();

                const string sql = @"SELECT DISTINCT e.EmployeeId as WarrantyRepresentativeEmployeeId, e.EmployeeNumber, LOWER(e.EmployeeName) as EmployeeName from CommunityAssignments ca
                                    INNER join Communities c
                                    ON ca.CommunityId = c.CommunityId
                                    INNER join Employees e
                                    ON ca.EmployeeId = e.EmployeeId
                                    INNER JOIN Cities ci
                                    ON c.CityId = ci.CityId
                                    WHERE CityCode IN ({0})
                                    ORDER BY EmployeeName";

                var result = _database.Fetch<AchievementReportModel.EmployeeTiedToRepresentative>(string.Format(sql, user.Markets.CommaSeparateWrapWithSingleQuote()));
                return result;
            }
        }
    }
}