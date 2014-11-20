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

        public SaltlineReportQueryHandler(IDatabase database, IUserSession userSession, IWarrantyCalculator warrantyCalculator )
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
            var divisions = GetDivisionsForReport();
            var projects = GetProjectsForReport();

            foreach (var employee in employees)
            {
                var employeeMonthlySaltlineSummary = GetEmployeeSaltlineSummary(query, employee.EmployeeNumber);
                employeeMonthlySaltlineSummary.EmployeeName = employee.EmployeeName;
                employeeMonthlySaltlineSummary.EmployeeNumber = employee.EmployeeNumber;
                model.EmployeeSaltlineSummary.Add(employeeMonthlySaltlineSummary);
            }

            foreach (var division in divisions)
            {
                var divisionSaltlineSummary = GetDivisionSaltlineSummary(query, division.DivisionName);
                divisionSaltlineSummary.DivisionName = division.DivisionName;
                divisionSaltlineSummary.DivisionId = division.DivisionCode;
                model.DivisionSaltlineSummary.Add(divisionSaltlineSummary);
            }

            foreach (var project in projects)
            {
                var projectSaltlineSummary = GetProjectSaltlineSummary(query, project.ProjectName);
                projectSaltlineSummary.ProjectName = project.ProjectName;
                projectSaltlineSummary.ProjectNumber = project.ProojectNumber;
                model.ProjectSaltlineSummary.Add(projectSaltlineSummary);
            }

            return model;
        }

        private SaltlineReportModel.SaltlineSummary GetEmployeeSaltlineSummary(SaltlineReportQuery query, string employeeNumber)
        {
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

        private SaltlineReportModel.SaltlineSummary GetDivisionSaltlineSummary(SaltlineReportQuery query, string divisionName)
        {
            var startDate = query.queryModel.StartDate.Value;
            var endDate = query.queryModel.EndDate.Value.ToLastDay();

            var monthRange = _warrantyCalculator.GetMonthRange(startDate, endDate);

            var excellentService = _warrantyCalculator.GetDivisionExcellentWarrantyService(startDate, endDate, divisionName);
            var definetelyWouldRecommend = _warrantyCalculator.GetDivisionDefinetelyWouldRecommend(startDate, endDate, divisionName);
            var rightTheFirstTime = _warrantyCalculator.GetDivisionRightTheFirstTime(startDate, endDate, divisionName);
            var amountSpent = _warrantyCalculator.GetDivisionAmountSpent(startDate, endDate, divisionName);
            var averageDays = _warrantyCalculator.GetDivisionAverageDaysClosed(startDate, endDate, divisionName);
            var percentClosedWithin7Days = _warrantyCalculator.GetDivisionPercentClosedWithin7Days(startDate, endDate, divisionName);

            return AgregateDataForReport(averageDays, percentClosedWithin7Days, amountSpent, excellentService, definetelyWouldRecommend, rightTheFirstTime, monthRange);
        }

        private SaltlineReportModel.SaltlineSummary GetProjectSaltlineSummary(SaltlineReportQuery query, string projectName)
        {
            var startDate = query.queryModel.StartDate.Value;
            var endDate = query.queryModel.EndDate.Value.ToLastDay();

            var monthRange = _warrantyCalculator.GetMonthRange(startDate, endDate);

            var excellentService = _warrantyCalculator.GetProjectExcellentWarrantyService(startDate, endDate, projectName);
            var definetelyWouldRecommend = _warrantyCalculator.GetProjectDefinetelyWouldRecommend(startDate, endDate, projectName);
            var rightTheFirstTime = _warrantyCalculator.GetProjectRightTheFirstTime(startDate, endDate, projectName);
            var amountSpent = _warrantyCalculator.GetProjectAmountSpent(startDate, endDate, projectName);
            var averageDays = _warrantyCalculator.GetProjectAverageDaysClosed(startDate, endDate, projectName);
            var percentClosedWithin7Days = _warrantyCalculator.GetProjectPercentClosedWithin7Days(startDate, endDate, projectName);

            return AgregateDataForReport(averageDays, percentClosedWithin7Days, amountSpent, excellentService, definetelyWouldRecommend, rightTheFirstTime, monthRange);
        }

        private SaltlineReportModel.SaltlineSummary AgregateDataForReport(IEnumerable<CalculatorResult> averageDays, IEnumerable<CalculatorResult> percentClosedWithin7Days, IEnumerable<CalculatorResult> amountSpent, IEnumerable<CalculatorResult> excellentService, IEnumerable<CalculatorResult> definetelyWouldRecommend, IEnumerable<CalculatorResult> rightTheFirstTime, IEnumerable<MonthYearModel> monthRange)
        {
            var list = new List<SaltlineReportModel.SaltlineSummary>();
            
            foreach (var range in monthRange)
            {
                list.Add(new SaltlineReportModel.SaltlineSummary
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
            return new SaltlineReportModel.SaltlineSummary
            {
                AmountSpentPerHome = list.Average(x => x.AmountSpentPerHome),
                AverageDaysClosing = list.Average(x => x.AverageDaysClosing),
                DefinetelyWouldRecommend = list.Average(x => x.DefinetelyWouldRecommend),
                ExcellentWarrantyService = list.Average(x => x.ExcellentWarrantyService),
                RightTheFirstTime = list.Average(x => x.RightTheFirstTime),
                PercentComplete7Days = list.Average(x => x.PercentComplete7Days),
            };
        }

        private decimal GetValueForMonth(IEnumerable<CalculatorResult> results, MonthYearModel range)
        {
            var result =  results.SingleOrDefault(x => x.MonthNumber == range.MonthNumber && x.YearNumber == range.YearNumber);
            return result != null ? result.Amount : 0;
        }

        private IEnumerable<SaltlineReportModel.EmployeeModel> GetEmployeesForReport()
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