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

        private AchievementReportModel.AchievementSummary GetPeriodAchievementSummary(SurveyReportData surveyReportData)
        {
            var def = (surveyReportData.DefinitelyWouldRecommend.Sum(w => w.TotalCalculableElements) /
                      surveyReportData.DefinitelyWouldRecommend.Sum(w => w.TotalElements)) * 100;

            var outs = (surveyReportData.OutstandingService.Sum(w => w.TotalCalculableElements) /
                      surveyReportData.OutstandingService.Sum(w => w.TotalElements)) * 100;
            
            var right = (surveyReportData.RightTheFirstTime.Sum(w => w.TotalCalculableElements) /
                      surveyReportData.RightTheFirstTime.Sum(w => w.TotalElements)) * 100;

            return new AchievementReportModel.AchievementSummary
                {
                    AmountSpentPerHome = surveyReportData.AchievementSummaries.Average(x => x.AmountSpentPerHome),
                    AverageDaysClosing = surveyReportData.AchievementSummaries.Average(x => x.AverageDaysClosing),
                    DefinitelyWouldRecommend = def,
                    OutstandingWarrantyService = outs,
                    RightTheFirstTime = right,
                    PercentComplete7Days = surveyReportData.AchievementSummaries.Average(x => x.PercentComplete7Days),
                };
        }

        private SurveyReportData GetMonthlyAchievementSummary(AchievementReportQuery query)
        {
            var surveyReportData = new SurveyReportData();

            var employeeNumber = query.queryModel.SelectedEmployeeNumber;
            var startDate = query.queryModel.StartDate.Value;
            var endDate = query.queryModel.EndDate.Value.ToLastDay();

            var monthRange = _warrantyCalculator.GetMonthRange(startDate, endDate);

            var surveyResults = _warrantyCalculator.GetDivisionSurveyData(startDate, endDate, employeeNumber).ToList();

            surveyReportData.OutstandingService = _warrantyCalculator.GetOutstandingWarrantyResults(surveyResults);
            surveyReportData.DefinitelyWouldRecommend = _warrantyCalculator.GetDefinitelyWouldRecommend(surveyResults);
            surveyReportData.RightTheFirstTime = _warrantyCalculator.GetRightTheFirstTimeWarrantyResults(surveyResults);
            surveyReportData.AmountSpent = _warrantyCalculator.GetEmployeeAmountSpent(startDate, endDate, employeeNumber);
            surveyReportData.AverageDays = _warrantyCalculator.GetEmployeeAverageDaysClosed(startDate, endDate, employeeNumber);
            surveyReportData.PercentClosedWithin7Days = _warrantyCalculator.GetEmployeePercentClosedWithin7Days(startDate, endDate, employeeNumber);
            surveyReportData.AchievementSummaries = AgregateDataForReport(surveyReportData, monthRange);

            return surveyReportData;
        }

        private IEnumerable<AchievementReportModel.AchievementSummary> AgregateDataForReport(SurveyReportData surveyReportData, IEnumerable<MonthYearModel> monthRange)
        {
            var list = new List<AchievementReportModel.AchievementSummary>();

            foreach (var range in monthRange)
            {
                list.Add(new AchievementReportModel.AchievementSummary
                    {
                        AverageDaysClosing = GetValueForMonth(surveyReportData.AverageDays, range) ?? 0,
                        PercentComplete7Days = GetValueForMonth(surveyReportData.PercentClosedWithin7Days, range) ?? 0,
                        AmountSpentPerHome = GetValueForMonth(surveyReportData.AmountSpent, range) ?? 0,
                        OutstandingWarrantyService = GetValueForMonth(surveyReportData.OutstandingService, range),
                        DefinitelyWouldRecommend = GetValueForMonth(surveyReportData.DefinitelyWouldRecommend, range),
                        RightTheFirstTime = GetValueForMonth(surveyReportData.RightTheFirstTime, range),
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