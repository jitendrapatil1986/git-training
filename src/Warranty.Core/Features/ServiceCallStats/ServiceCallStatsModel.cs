namespace Warranty.Core.Features.ServiceCallStats
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Enumerations;

    public class ServiceCallStatsModel
    {
        public IEnumerable<LineItem> LineItems { get; set; }
        public StatView View { get; set; }

        public int CurrentQuarter { get; set; }
        public string[] Months { get; set; }
        public List<Series<decimal>> DollarsSpentSeriesList { get; set; }
        public List<Series<int>> AverageDaysClosedSeriesList { get; set; }
        public List<Series<decimal>> PercentClosedSeriesList { get; set; }

        public string ShowLegend { get { return (LineItems.Count() > 1).ToString().ToLower(); }}

        public class LineItem
        {
            public int AverageDaysClosed { get; set; }
            public decimal PercentClosedWithinSevenDays { get; set; }
            public decimal TotalDollarsSpent { get; set; }
            public int NumberOfWarrantableHomes { get; set; }
            public decimal DollarsSpentPerHome { get; set; }
            public Guid EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public string EmployeeNumber { get; set; }
            public string CityCode { get; set; }            
        }

        public class Series<T>
        {
            public string Name { get; set; }
            public List<T> Data { get; set; }
        }
    }
}
