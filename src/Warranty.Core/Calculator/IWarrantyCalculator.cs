namespace Warranty.Core.Calculator
{
    using System;
    using System.Collections.Generic;
    using Security;

    public interface IWarrantyCalculator
    {
        IEnumerable<CalculatorResult> GetAverageDaysClosed(DateTime startDate, DateTime endDate, string employeeNumber);
        IEnumerable<CalculatorResult> GetPercentClosedWithin7Days(DateTime startDate, DateTime endDate, string employeeNumber);
        IEnumerable<CalculatorResult> GetAmountSpent(DateTime startDate, DateTime endDate, string employeeNumber);
        IEnumerable<CalculatorResult> GetExcellentWarrantyService(DateTime startDate, DateTime endDate, string employeeNumber);
        IEnumerable<CalculatorResult> GetRightTheFirstTime(DateTime startDate, DateTime endDate, string employeeNumber);
        IEnumerable<CalculatorResult> GetDefinetelyWouldRecommend(DateTime startDate, DateTime endDate, string employeeNumber);
    }
}