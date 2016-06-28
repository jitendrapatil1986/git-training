namespace Warranty.Core.Calculator
{
    public class CalculatorResult : MonthYearModel
    {
        public static readonly CalculatorResult Default = new CalculatorResult
        {
            Amount = 0,
            TotalElements = 0,
            TotalCalculableElements = 0,
        };

        public decimal? Amount { get; set; }
        public decimal TotalElements { get; set; }
        public decimal TotalCalculableElements { get; set; }
    }
}
