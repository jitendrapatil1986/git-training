namespace Warranty.Core.Calculator
{
    using System;
    using System.Collections.Generic;

    public interface IWarrantyCalculator
    {
        IEnumerable<CalculatorResult> GetEmployeeAverageDaysClosed(DateTime startDate, DateTime endDate, string employeeNumber);
        IEnumerable<CalculatorResult> GetEmployeePercentClosedWithin7Days(DateTime startDate, DateTime endDate, string employeeNumber);
        IEnumerable<CalculatorResult> GetEmployeeAmountSpent(DateTime startDate, DateTime endDate, string employeeNumber);
        IEnumerable<CalculatorResult> GetEmployeeExcellentWarrantyService(DateTime startDate, DateTime endDate, string employeeNumber);
        IEnumerable<CalculatorResult> GetEmployeeRightTheFirstTime(DateTime startDate, DateTime endDate, string employeeNumber);
        IEnumerable<CalculatorResult> GetEmployeeDefinetelyWouldRecommend(DateTime startDate, DateTime endDate, string employeeNumber);
        IEnumerable<CalculatorResult> GetEmployeeWarrantableHomes(DateTime startDate, DateTime endDate, string employeeNumber);

        IEnumerable<CalculatorResult> GetDivisionAverageDaysClosed(DateTime startDate, DateTime endDate, string divisionName);
        IEnumerable<CalculatorResult> GetDivisionPercentClosedWithin7Days(DateTime startDate, DateTime endDate, string divisionName);
        IEnumerable<CalculatorResult> GetDivisionAmountSpent(DateTime startDate, DateTime endDate, string divisionName);
        IEnumerable<CalculatorResult> GetDivisionExcellentWarrantyService(DateTime startDate, DateTime endDate, string divisionName);
        IEnumerable<CalculatorResult> GetDivisionRightTheFirstTime(DateTime startDate, DateTime endDate, string divisionName);
        IEnumerable<CalculatorResult> GetDivisionDefinetelyWouldRecommend(DateTime startDate, DateTime endDate, string divisionName);
        IEnumerable<CalculatorResult> GetDivisionWarrantableHomes(DateTime startDate, DateTime endDate, string divisionName);

        IEnumerable<CalculatorResult> GetProjectAverageDaysClosed(DateTime startDate, DateTime endDate, string projectName);
        IEnumerable<CalculatorResult> GetProjectPercentClosedWithin7Days(DateTime startDate, DateTime endDate, string projectName);
        IEnumerable<CalculatorResult> GetProjectAmountSpent(DateTime startDate, DateTime endDate, string projectName);
        IEnumerable<CalculatorResult> GetProjectExcellentWarrantyService(DateTime startDate, DateTime endDate, string projectName);
        IEnumerable<CalculatorResult> GetProjectRightTheFirstTime(DateTime startDate, DateTime endDate, string projectName);
        IEnumerable<CalculatorResult> GetProjectDefinetelyWouldRecommend(DateTime startDate, DateTime endDate, string projectName);
        IEnumerable<CalculatorResult> GetProjectWarrantableHomes(DateTime startDate, DateTime endDate, string projectName);

        IEnumerable<MonthYearModel> GetMonthRange(DateTime startDate, DateTime endDate);
    }
}