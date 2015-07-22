using System.Collections.Generic;
using Warranty.Core.Features.Report.Achievement;

namespace Warranty.Core.Calculator
{
    public class SurveyReportData
    {
        public IEnumerable<CalculatorResult> DefinitelyWouldRecommend { get; set; }
        public IEnumerable<CalculatorResult> RightTheFirstTime { get; set; }
        public IEnumerable<CalculatorResult> OutstandingService { get; set; }
        public IEnumerable<CalculatorResult> AverageDays { get; set; }
        public IEnumerable<CalculatorResult> PercentClosedWithin7Days { get; set; }
        public IEnumerable<CalculatorResult> AmountSpent { get; set; }
        public IEnumerable<AchievementReportModel.AchievementSummary> AchievementSummaries { get; set; }
    }
}
