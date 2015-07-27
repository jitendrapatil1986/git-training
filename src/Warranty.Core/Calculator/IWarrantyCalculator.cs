namespace Warranty.Core.Calculator
{
    using System;
    using System.Collections.Generic;

    public interface IWarrantyCalculator
    {
        IEnumerable<CalculatorResult> GetEmployeeAverageDaysClosed(DateTime startDate, DateTime endDate, string employeeNumber);
        IEnumerable<CalculatorResult> GetEmployeePercentClosedWithin7Days(DateTime startDate, DateTime endDate, string employeeNumber);
        IEnumerable<CalculatorResult> GetEmployeeAmountSpent(DateTime startDate, DateTime endDate, string employeeNumber);
        IEnumerable<CalculatorResult> GetEmployeeOutstandingWarrantyService(DateTime startDate, DateTime endDate, string employeeNumber);
        IEnumerable<CalculatorResult> GetEmployeeRightTheFirstTime(DateTime startDate, DateTime endDate, string employeeNumber);
        IEnumerable<CalculatorResult> GetEmployeeDefinitelyWouldRecommend(DateTime startDate, DateTime endDate, string employeeNumber);
        IEnumerable<CalculatorResult> GetEmployeeWarrantableHomes(DateTime startDate, DateTime endDate, string employeeNumber);

        IEnumerable<CalculatorResult> GetDivisionAverageDaysClosed(DateTime startDate, DateTime endDate, string divisionName);
        IEnumerable<CalculatorResult> GetDivisionPercentClosedWithin7Days(DateTime startDate, DateTime endDate, string divisionName);
        IEnumerable<CalculatorResult> GetDivisionAmountSpent(DateTime startDate, DateTime endDate, string divisionName);
        IEnumerable<CalculatorResult> GetDivisionOutstandingWarrantyService(DateTime startDate, DateTime endDate, string divisionName);
        IEnumerable<CalculatorResult> GetDivisionRightTheFirstTime(DateTime startDate, DateTime endDate, string divisionName);
        IEnumerable<CalculatorResult> GetDivisionDefinitelyWouldRecommend(DateTime startDate, DateTime endDate, string divisionName);
        IEnumerable<CalculatorResult> GetDivisionWarrantableHomes(DateTime startDate, DateTime endDate, string divisionName);

        IEnumerable<CalculatorResult> GetProjectAverageDaysClosed(DateTime startDate, DateTime endDate, string projectName);
        IEnumerable<CalculatorResult> GetProjectPercentClosedWithin7Days(DateTime startDate, DateTime endDate, string projectName);
        IEnumerable<CalculatorResult> GetProjectAmountSpent(DateTime startDate, DateTime endDate, string projectName);
        IEnumerable<CalculatorResult> GetProjectOutstandingWarrantyService(DateTime startDate, DateTime endDate, string projectName);
        IEnumerable<CalculatorResult> GetProjectRightTheFirstTime(DateTime startDate, DateTime endDate, string projectName);
        IEnumerable<CalculatorResult> GetProjectDefinitelyWouldRecommend(DateTime startDate, DateTime endDate, string projectName);
        IEnumerable<CalculatorResult> GetProjectWarrantableHomes(DateTime startDate, DateTime endDate, string projectName);

        IEnumerable<MonthYearModel> GetMonthRange(DateTime startDate, DateTime endDate);
    }
}