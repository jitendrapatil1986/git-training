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
            var projects = GetProjectsForReport();
            var divisions = GetDivisionsForReport();

            foreach (var employee in employees.OrderBy(x=>x.EmployeeName))
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

            var outstandingService = _warrantyCalculator.GetEmployeeOutstandingWarrantyService(startDate, endDate, employeeNumber);
            var definitelyWouldRecommend = _warrantyCalculator.GetEmployeeDefinitelyWouldRecommend(startDate, endDate, employeeNumber);
            var rightTheFirstTime = _warrantyCalculator.GetEmployeeRightTheFirstTime(startDate, endDate, employeeNumber);
            var amountSpent = _warrantyCalculator.GetEmployeeAmountSpent(startDate, endDate, employeeNumber);
            var averageDays = _warrantyCalculator.GetEmployeeAverageDaysClosed(startDate, endDate, employeeNumber);
            var percentClosedWithin7Days = _warrantyCalculator.GetEmployeePercentClosedWithin7Days(startDate, endDate, employeeNumber);
            var numberOfHomes = _warrantyCalculator.GetEmployeeWarrantableHomes(startDate, endDate, employeeNumber);

            return AgregateDataForReport(averageDays, percentClosedWithin7Days, amountSpent, outstandingService, definitelyWouldRecommend, rightTheFirstTime, monthRange, numberOfHomes);
        }

        private SaltlineReportModel.SaltlineSummary GetDivisionSaltlineSummary(SaltlineReportQuery query, string divisionName)
        {
            var startDate = query.queryModel.StartDate.Value;
            var endDate = query.queryModel.EndDate.Value.ToLastDay();

            var monthRange = _warrantyCalculator.GetMonthRange(startDate, endDate);

            var outstandingService = _warrantyCalculator.GetDivisionOutstandingWarrantyService(startDate, endDate, divisionName);
            var definitelyWouldRecommend = _warrantyCalculator.GetDivisionDefinitelyWouldRecommend(startDate, endDate, divisionName);
            var rightTheFirstTime = _warrantyCalculator.GetDivisionRightTheFirstTime(startDate, endDate, divisionName);
            var amountSpent = _warrantyCalculator.GetDivisionAmountSpent(startDate, endDate, divisionName);
            var averageDays = _warrantyCalculator.GetDivisionAverageDaysClosed(startDate, endDate, divisionName);
            var percentClosedWithin7Days = _warrantyCalculator.GetDivisionPercentClosedWithin7Days(startDate, endDate, divisionName);
            var numberOfHomes = _warrantyCalculator.GetDivisionWarrantableHomes(startDate, endDate, divisionName);

            return AgregateDataForReport(averageDays, percentClosedWithin7Days, amountSpent, outstandingService, definitelyWouldRecommend, rightTheFirstTime, monthRange, numberOfHomes);
        }

        private SaltlineReportModel.SaltlineSummary GetProjectSaltlineSummary(SaltlineReportQuery query, string projectName)
        {
            var startDate = query.queryModel.StartDate.Value;
            var endDate = query.queryModel.EndDate.Value.ToLastDay();

            var monthRange = _warrantyCalculator.GetMonthRange(startDate, endDate);

            var outstandingService = _warrantyCalculator.GetProjectOutstandingWarrantyService(startDate, endDate, projectName);
            var definitelyWouldRecommend = _warrantyCalculator.GetProjectDefinitelyWouldRecommend(startDate, endDate, projectName);
            var rightTheFirstTime = _warrantyCalculator.GetProjectRightTheFirstTime(startDate, endDate, projectName);
            var amountSpent = _warrantyCalculator.GetProjectAmountSpent(startDate, endDate, projectName);
            var averageDays = _warrantyCalculator.GetProjectAverageDaysClosed(startDate, endDate, projectName);
            var percentClosedWithin7Days = _warrantyCalculator.GetProjectPercentClosedWithin7Days(startDate, endDate, projectName);
            var numberOfHomes = _warrantyCalculator.GetProjectWarrantableHomes(startDate, endDate, projectName);

            return AgregateDataForReport(averageDays, percentClosedWithin7Days, amountSpent, outstandingService, definitelyWouldRecommend, rightTheFirstTime, monthRange, numberOfHomes);
        }

        private SaltlineReportModel.SaltlineSummary AgregateDataForReport(IEnumerable<CalculatorResult> averageDays, IEnumerable<CalculatorResult> percentClosedWithin7Days, IEnumerable<CalculatorResult> amountSpent, IEnumerable<CalculatorResult> outstandingService, IEnumerable<CalculatorResult> definitelyWouldRecommend, IEnumerable<CalculatorResult> rightTheFirstTime, IEnumerable<MonthYearModel> monthRange, IEnumerable<CalculatorResult> numberOfHomes)
        {
            var list = new List<SaltlineReportModel.SaltlineSummary>();
            
            foreach (var range in monthRange)
            {
                list.Add(new SaltlineReportModel.SaltlineSummary
                    {
                        AverageDaysClosing = GetValueForMonth(averageDays, range) ?? 0,
                        PercentComplete7Days = GetValueForMonth(percentClosedWithin7Days, range) ?? 0,
                        AmountSpentPerHome = GetValueForMonth(amountSpent, range) ?? 0,
                        OutstandingWarrantyService = GetValueForMonth(outstandingService, range),
                        DefinitelyWouldRecommend = GetValueForMonth(definitelyWouldRecommend, range),
                        RightTheFirstTime = GetValueForMonth(rightTheFirstTime, range),
                        Month = range.MonthNumber,
                        Year = range.YearNumber,
                        NumerOfCalls = averageDays.Sum(x=>x.TotalElements),
                        NumberOfSurveys = outstandingService.Sum(x => x.TotalElements),
                        NumberOfHomes = GetTotalElementsForMonth(numberOfHomes, range).GetValueOrDefault(),
                    });
            }
            return new SaltlineReportModel.SaltlineSummary
            {
                AmountSpentPerHome = list.Average(x => x.AmountSpentPerHome),
                AverageDaysClosing = list.Average(x => x.AverageDaysClosing),
                DefinitelyWouldRecommend = list.Where(x=>x.DefinitelyWouldRecommend != null).Average(x => x.DefinitelyWouldRecommend),
                OutstandingWarrantyService = list.Where(x=>x.OutstandingWarrantyService != null).Average(x => x.OutstandingWarrantyService),
                RightTheFirstTime = list.Where(x=>x.RightTheFirstTime != null).Average(x => x.RightTheFirstTime),
                PercentComplete7Days = list.Average(x => x.PercentComplete7Days),
                NumerOfCalls = averageDays.Sum(x => x.TotalElements),
                NumberOfSurveys = outstandingService.Sum(x => x.TotalElements),
                NumberOfHomes = numberOfHomes.Sum(x => x.TotalElements),
            };
        }

        private decimal? GetValueForMonth(IEnumerable<CalculatorResult> results, MonthYearModel range)
        {
            var result =  results.SingleOrDefault(x => x.MonthNumber == range.MonthNumber && x.YearNumber == range.YearNumber);
            return result != null ? result.Amount.Value : (decimal?)null;
        }        
        
        private decimal? GetTotalElementsForMonth(IEnumerable<CalculatorResult> results, MonthYearModel range)
        {
            var result =  results.SingleOrDefault(x => x.MonthNumber == range.MonthNumber && x.YearNumber == range.YearNumber);
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