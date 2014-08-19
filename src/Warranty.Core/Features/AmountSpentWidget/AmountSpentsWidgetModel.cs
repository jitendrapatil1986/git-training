using System.Collections.Generic;

namespace Warranty.Core.Features.AmountSpentWidget
{
    public class AmountSpentWidgetModel
    {
        public string[] Categories { get; set; }
        public List<Series> SeriesList { get; set; }
        public decimal YearTodate { get; set; }
        public decimal MonthTodate { get; set; }
        public decimal QuarterToDate { get; set; }

        public class Series
        {
            public string Name { get; set; }
            public List<decimal> Data { get; set; }
        }
    }
}
