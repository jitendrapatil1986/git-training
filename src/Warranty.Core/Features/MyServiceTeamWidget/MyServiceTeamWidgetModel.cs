namespace Warranty.Core.Features.MyServiceTeamWidget
{
    using System;
    using System.Collections.Generic;

    public class MyServiceTeamWidgetModel
    {
        public string Months { get; set; }
        public string SeriesDataOpen { get; set; }
        public string SeriesDataClosed { get; set; }
        public string SeriesDataOverdue { get; set; }
        public IEnumerable<MyTeamChartEmployeeSummary> MyTeamChartEmployeeSummaries { get; set; }

        public class MyTeamChartEmployeeDetail: MyTeamChartData
        {            
            public int MonthNumber { get; set; }
            public string MonthName { get; set; }
        }

        public class MyTeamChartEmployeeSummary: MyTeamChartData
        {
            public int Year { get; set; }
        }

        public class MyTeamMonth
        {
            public int Month { get; set; }
            public string MonthName { get; set; }
        }

        public class MyTeamChartData
        {
            public Guid WarrantyRepresentativeEmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public int TotalCalls { get; set; }
            public int TotalOpen { get; set; }
            public int TotalClosed { get; set; }
            public int TotalOverdue { get; set; }
        }
    }
}
