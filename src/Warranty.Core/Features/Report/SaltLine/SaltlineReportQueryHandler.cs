namespace Warranty.Core.Features.Report.Saltline
{
    using System.Collections.Generic;
    using System.Linq;
    using Calculator;
    using Extensions;
    using NPoco;
    using Security;

    public class SaltlineReportQueryHandler : IQueryHandler<SaltlineReportQuery, SaltlineReportModel>
    {
        private readonly IDatabase _database;
        private readonly IUserSession _userSession;
        private readonly IWarrantyCalculator _warrantyCalculator;

        public SaltlineReportQueryHandler(IDatabase database, IUserSession userSession, IWarrantyCalculator warrantyCalculator)
        {
            _database = database;
            _userSession = userSession;
            _warrantyCalculator = warrantyCalculator;
        }

        public SaltlineReportModel Handle(SaltlineReportQuery query)
        {
            var model = new SaltlineReportModel();

            if (!query.queryModel.HasSearchCriteria)
                return model;

            var employees = GetEmployeesForReport();
            var projects = GetProjectsForReport();
            var divisions = GetDivisionsForReport();

            foreach (var employee in employees.OrderBy(x => x.EmployeeName))
            {
                var employeeMonthlySaltlineSummary = GetEmployeeSaltlineSummary(query, employee.EmployeeNumber);
                employeeMonthlySaltlineSummary.EmployeeName = employee.EmployeeName;
                employeeMonthlySaltlineSummary.EmployeeNumber = employee.EmployeeNumber;
                model.EmployeeSaltlineSummary.Add(employeeMonthlySaltlineSummary);
            }

            foreach (var project in projects.OrderBy(x => x.ProjectName))
            {
                var projectSaltlineSummary = GetProjectSaltlineSummary(query, project.ProjectName);
                projectSaltlineSummary.ProjectName = project.ProjectName;
                projectSaltlineSummary.ProjectNumber = project.ProjectNumber;
                model.ProjectSaltlineSummary.Add(projectSaltlineSummary);
            }

            foreach (var division in divisions.OrderBy(x => x.DivisionName))
            {
                var divisionSaltlineSummary = GetDivisionSaltlineSummary(query, division.DivisionName);
                divisionSaltlineSummary.DivisionName = division.DivisionName;
                divisionSaltlineSummary.DivisionId = division.DivisionCode;
                if (!model.DivisionSaltlineSummary.Any(x => x.DivisionName == divisionSaltlineSummary.DivisionName && x.NumberOfSurveys == divisionSaltlineSummary.NumberOfSurveys && x.DefinitelyWouldRecommend == divisionSaltlineSummary.DefinitelyWouldRecommend && x.OutstandingWarrantyService == divisionSaltlineSummary.OutstandingWarrantyService))
                {
                    model.DivisionSaltlineSummary.Add(divisionSaltlineSummary);
                }
            }

            return model;
        }

        private SaltlineReportModel.SaltlineSummary GetEmployeeSaltlineSummary(SaltlineReportQuery query, string employeeNumber)
        {
            var startDate = query.queryModel.StartDate.Value;
            var endDate = query.queryModel.EndDate.Value.ToLastDay();

            var monthRange = _warrantyCalculator.GetMonthRange(startDate, endDate);


            var surveyResults = _warrantyCalculator.GetEmployeeSurveyData(startDate, endDate, employeeNumber).ToList();
            var surveyReportData = new SurveyReportData
            {
                OutstandingService = _warrantyCalculator.GetOutstandingWarrantyResults(surveyResults),
                DefinitelyWouldRecommend = _warrantyCalculator.GetDefinitelyWouldRecommend(surveyResults),
                RightTheFirstTime = _warrantyCalculator.GetRightTheFirstTimeWarrantyResults(surveyResults),
                AmountSpent = _warrantyCalculator.GetEmployeeAmountSpent(startDate, endDate, employeeNumber),
                AverageDays = _warrantyCalculator.GetEmployeeAverageDaysClosed(startDate, endDate, employeeNumber),
                PercentClosedWithin7Days = _warrantyCalculator.GetEmployeePercentClosedWithin7Days(startDate, endDate, employeeNumber)
            };
            var numberOfHomes = _warrantyCalculator.GetEmployeeWarrantableHomes(startDate, endDate, employeeNumber);

            return AgregateDataForReport(surveyReportData, monthRange, numberOfHomes);
        }

        private SaltlineReportModel.SaltlineSummary GetDivisionSaltlineSummary(SaltlineReportQuery query, string divisionName)
        {
            var startDate = query.queryModel.StartDate.Value;
            var endDate = query.queryModel.EndDate.Value.ToLastDay();

            var monthRange = _warrantyCalculator.GetMonthRange(startDate, endDate);
            var surveyResults = _warrantyCalculator.GetDivisionSurveyData(startDate, endDate, divisionName).ToList();

            var surveyReportData = new SurveyReportData
            {
                OutstandingService = _warrantyCalculator.GetOutstandingWarrantyResults(surveyResults),
                DefinitelyWouldRecommend = _warrantyCalculator.GetDefinitelyWouldRecommend(surveyResults),
                RightTheFirstTime = _warrantyCalculator.GetRightTheFirstTimeWarrantyResults(surveyResults),
                AmountSpent = _warrantyCalculator.GetDivisionAmountSpent(startDate, endDate, divisionName),
                AverageDays = _warrantyCalculator.GetDivisionAverageDaysClosed(startDate, endDate, divisionName),
                PercentClosedWithin7Days = _warrantyCalculator.GetDivisionPercentClosedWithin7Days(startDate, endDate, divisionName),
            };
            var numberOfHomes = _warrantyCalculator.GetDivisionWarrantableHomes(startDate, endDate, divisionName);

            return AgregateDataForReport(surveyReportData, monthRange, numberOfHomes);
        }

        private SaltlineReportModel.SaltlineSummary GetProjectSaltlineSummary(SaltlineReportQuery query, string projectName)
        {
            var startDate = query.queryModel.StartDate.Value;
            var endDate = query.queryModel.EndDate.Value.ToLastDay();

            var monthRange = _warrantyCalculator.GetMonthRange(startDate, endDate);
            var surveyResults = _warrantyCalculator.GetProjectSurveyData(startDate, endDate, projectName).ToList();

            var surveyReportData = new SurveyReportData
            {
                OutstandingService = _warrantyCalculator.GetOutstandingWarrantyResults(surveyResults),
                DefinitelyWouldRecommend = _warrantyCalculator.GetDefinitelyWouldRecommend(surveyResults),
                RightTheFirstTime = _warrantyCalculator.GetRightTheFirstTimeWarrantyResults(surveyResults),
                AmountSpent = _warrantyCalculator.GetProjectAmountSpent(startDate, endDate, projectName),
                AverageDays = _warrantyCalculator.GetProjectAverageDaysClosed(startDate, endDate, projectName),
                PercentClosedWithin7Days = _warrantyCalculator.GetProjectPercentClosedWithin7Days(startDate, endDate, projectName),
            };
            var numberOfHomes = _warrantyCalculator.GetProjectWarrantableHomes(startDate, endDate, projectName);

            return AgregateDataForReport(surveyReportData, monthRange, numberOfHomes);
        }

        private SaltlineReportModel.SaltlineSummary AgregateDataForReport(SurveyReportData surveyReportData, IEnumerable<MonthYearModel> monthRange, IEnumerable<CalculatorResult> numberOfHomes)
        {
            var list = new List<SaltlineReportModel.SaltlineSummary>();

            foreach (var range in monthRange)
            {
                list.Add(new SaltlineReportModel.SaltlineSummary
                    {
                        AverageDaysClosing = GetValueForMonth(surveyReportData.AverageDays, range) ?? 0,
                        PercentComplete7Days = GetValueForMonth(surveyReportData.PercentClosedWithin7Days, range) ?? 0,
                        AmountSpentPerHome = GetValueForMonth(surveyReportData.AmountSpent, range) ?? 0,
                        OutstandingWarrantyService = GetValueForMonth(surveyReportData.OutstandingService, range),
                        DefinitelyWouldRecommend = GetValueForMonth(surveyReportData.DefinitelyWouldRecommend, range),
                        RightTheFirstTime = GetValueForMonth(surveyReportData.RightTheFirstTime, range),
                        Month = range.MonthNumber,
                        Year = range.YearNumber,
                        NumerOfCalls = surveyReportData.AverageDays.Sum(x => x.TotalElements),
                        NumberOfSurveys = surveyReportData.OutstandingService.Sum(x => x.TotalElements),
                        NumberOfHomes = GetTotalElementsForMonth(numberOfHomes, range).GetValueOrDefault(),
                    });
            }

            var def = surveyReportData.DefinitelyWouldRecommend.Any()
                ? (surveyReportData.DefinitelyWouldRecommend.Sum(w => w.TotalCalculableElements) /
                surveyReportData.DefinitelyWouldRecommend.Sum(w => w.TotalElements)) * 100
                : 0;

            var outs = surveyReportData.OutstandingService.Any()
                ? (surveyReportData.OutstandingService.Sum(w => w.TotalCalculableElements) /
                surveyReportData.OutstandingService.Sum(w => w.TotalElements)) * 100
                : 0;

            var right = surveyReportData.RightTheFirstTime.Any()
                ? (surveyReportData.RightTheFirstTime.Sum(w => w.TotalCalculableElements) /
                surveyReportData.RightTheFirstTime.Sum(w => w.TotalElements)) * 100
                : 0;

            return new SaltlineReportModel.SaltlineSummary
            {
                AmountSpentPerHome = list.Average(x => x.AmountSpentPerHome),
                AverageDaysClosing = list.Average(x => x.AverageDaysClosing),
                DefinitelyWouldRecommend = def,
                OutstandingWarrantyService = outs,
                RightTheFirstTime = right,
                PercentComplete7Days = list.Average(x => x.PercentComplete7Days),
                NumerOfCalls = surveyReportData.AverageDays.Sum(x => x.TotalElements),
                NumberOfSurveys = surveyReportData.OutstandingService.Sum(x => x.TotalElements),
                NumberOfHomes = numberOfHomes.Sum(x => x.TotalElements),
            };
        }

        private decimal? GetValueForMonth(IEnumerable<CalculatorResult> results, MonthYearModel range)
        {
            var result = results.SingleOrDefault(x => x.MonthNumber == range.MonthNumber && x.YearNumber == range.YearNumber);
            return result != null ? result.Amount.Value : (decimal?)null;
        }

        private decimal? GetTotalElementsForMonth(IEnumerable<CalculatorResult> results, MonthYearModel range)
        {
            var result = results.SingleOrDefault(x => x.MonthNumber == range.MonthNumber && x.YearNumber == range.YearNumber);
            return result != null ? result.TotalElements : (decimal?)null;
        }

        private IEnumerable<SaltlineReportModel.EmployeeModel> GetEmployeesForReport()
        {
            using (_database)
            {
                var user = _userSession.GetCurrentUser();

                const string sql = @"SELECT DISTINCT e.EmployeeId as WarrantyRepresentativeEmployeeId, e.EmployeeNumber, LOWER(e.EmployeeName) as EmployeeName 
                                    from CommunityAssignments ca
                                    INNER join Communities c
                                    ON ca.CommunityId = c.CommunityId
                                    INNER join Employees e
                                    ON ca.EmployeeId = e.EmployeeId
                                    INNER JOIN Cities ci
                                    ON c.CityId = ci.CityId
                                    WHERE CityCode IN ({0})
                                    ORDER BY EmployeeName";

                var result = _database.Fetch<SaltlineReportModel.EmployeeModel>(string.Format(sql, user.Markets.CommaSeparateWrapWithSingleQuote()));
                return result;
            }
        }

        private IEnumerable<SaltlineReportModel.DivisionModel> GetDivisionsForReport()
        {
            using (_database)
            {
                var user = _userSession.GetCurrentUser();

                const string sql =
                    @"SELECT DISTINCT d.DivisionCode, d.DivisionName                                        
                                    FROM Communities cm
                                    INNER JOIN Divisions d
                                    ON cm.DivisionId = d.DivisionId
                                        INNER JOIN Cities ci
                                    ON cm.CityId = ci.CityId
                                    WHERE CityCode IN ({0})";

                var result = _database.Fetch<SaltlineReportModel.DivisionModel>(string.Format(sql, user.Markets.CommaSeparateWrapWithSingleQuote()));
                return result;
            }
        }

        private IEnumerable<SaltlineReportModel.ProjectModel> GetProjectsForReport()
        {
            using (_database)
            {
                var user = _userSession.GetCurrentUser();

                const string sql =
                    @"SELECT DISTINCT pr.ProjectNumber, pr.ProjectName                                        
                                    FROM Communities cm
                                    INNER JOIN Projects pr
                                    ON cm.ProjectId = pr.ProjectId
                                        INNER JOIN Cities ci
                                    ON cm.CityId = ci.CityId
                                    WHERE CityCode IN ({0})";

                var result = _database.Fetch<SaltlineReportModel.ProjectModel>(string.Format(sql, user.Markets.CommaSeparateWrapWithSingleQuote()));
                return result;
            }
        }
    }
}