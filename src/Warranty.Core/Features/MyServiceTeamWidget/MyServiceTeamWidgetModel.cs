namespace Warranty.Core.Features.MyServiceTeamWidget
{
    using System;
    using System.Collections.Generic;

    public class MyServiceTeamWidgetModel
    {
        public IEnumerable<string> Months { get; set; }
        public IEnumerable<Series> SeriesDataOpen { get; set; }
        public IEnumerable<Series> SeriesDataClosed { get; set; }
        public IEnumerable<MyTeamChartData> MyTeamChartEmployeeSummaries { get; set; }

        public class MyTeamChartData
        {
            public int Month { get; set; }
            public Guid WarrantyRepresentativeEmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public int TotalCalls { get; set; }
            public int TotalOpen { get; set; }
            public int TotalClosed { get; set; }
        }

        public class Series
        {
            public string Name { get; set; }
            public List<int> Data { get; set; }
        }
    }
}
