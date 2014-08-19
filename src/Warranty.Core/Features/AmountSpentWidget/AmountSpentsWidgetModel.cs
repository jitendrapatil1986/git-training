using System.Collections.Generic;

namespace Warranty.Core.Features.AmountSpentWidget
{
    public class AmountSpentWidgetModel
    {
        public IEnumerable<string> Categories { get; set; }
        public List<Series> SeriesList { get; set; }
        public decimal YearToDate { get; set; }
        public decimal MonthToDate { get; set; }
        public decimal QuarterToDate { get; set; }

        public class Series
        {
            public string Name { get; set; }
            public List<decimal> Data { get; set; }
        }
    }
}
