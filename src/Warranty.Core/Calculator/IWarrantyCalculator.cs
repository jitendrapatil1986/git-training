namespace Warranty.Core.Calculator
{
    using System;
    using System.Collections.Generic;

    public interface IWarrantyCalculator
    {
        IEnumerable<CalculatorResult> GetEmployeeAverageDaysClosed(DateTime startDate, DateTime endDate, string employeeNumber);
        IEnumerable<CalculatorResult> GetEmployeePercentClosedWithin7Days(DateTime startDate, DateTime endDate, string employeeNumber);
        IEnumerable<CalculatorResult> GetEmployeeAmountSpent(DateTime startDate, DateTime endDate, string employeeNumber);
        IEnumerable<CalculatorResult> GetEmployeeWarrantableHomes(DateTime startDate, DateTime endDate, string employeeNumber);
        IEnumerable<CalculatorResult> GetEmployeeNumberOfServiceCallsOpen(DateTime endDate, string employeeNumber);

        IEnumerable<CalculatorResult> GetDivisionAverageDaysClosed(DateTime startDate, DateTime endDate, string divisionName);
        IEnumerable<CalculatorResult> GetDivisionPercentClosedWithin7Days(DateTime startDate, DateTime endDate, string divisionName);
        IEnumerable<CalculatorResult> GetDivisionAmountSpent(DateTime startDate, DateTime endDate, string divisionName);
        IEnumerable<CalculatorResult> GetDivisionWarrantableHomes(DateTime startDate, DateTime endDate, string divisionName);
        IEnumerable<CalculatorResult> GetDivisionNumberOfServiceCallsOpen(DateTime endDate, string divisionName);

        IEnumerable<CalculatorResult> GetProjectAverageDaysClosed(DateTime startDate, DateTime endDate, string projectName);
        IEnumerable<CalculatorResult> GetProjectPercentClosedWithin7Days(DateTime startDate, DateTime endDate, string projectName);
        IEnumerable<CalculatorResult> GetProjectAmountSpent(DateTime startDate, DateTime endDate, string projectName);
        IEnumerable<CalculatorResult> GetProjectWarrantableHomes(DateTime startDate, DateTime endDate, string projectName);
        IEnumerable<CalculatorResult> GetProjectNumberOfServiceCallsOpen(DateTime endDate, string projectName);


        IEnumerable<SurveyDataResult> GetEmployeeSurveyData(DateTime startDate, DateTime endDate, string employeeNumber);
        IEnumerable<SurveyDataResult> GetDivisionSurveyData(DateTime startDate, DateTime endDate, string divisionName);
        IEnumerable<SurveyDataResult> GetProjectSurveyData(DateTime startDate, DateTime endDate, string projectName);
        
        IEnumerable<CalculatorResult> GetOutstandingWarrantyResults(IEnumerable<SurveyDataResult> surveyData);
        IEnumerable<CalculatorResult> GetDefinitelyWouldRecommend(IEnumerable<SurveyDataResult> surveyData);
        IEnumerable<CalculatorResult> GetRightTheFirstTimeWarrantyResults(IEnumerable<SurveyDataResult> surveyData);

        IEnumerable<MonthYearModel> GetMonthRange(DateTime startDate, DateTime endDate);
    }
}