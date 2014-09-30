namespace Warranty.Core.Features.ServiceCallStats
{
    using System;
    using System.Collections.Generic;

    public class ServiceCallStatsModel
    {
        public IEnumerable<LineItem> LineItems { get; set; }

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
    }
}
