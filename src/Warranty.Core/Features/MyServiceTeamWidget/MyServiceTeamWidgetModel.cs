namespace Warranty.Core.Features.MyServiceTeamWidget
{
    using System;

    public class MyServiceTeamWidgetModel
    {
        public string Months { get; set; }
        public string SeriesData { get; set; }

        public class MyTeamChart
        {
            public Guid WarrantyRepresentativeEmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public int Month { get; set; }
            public string MonthName { get; set; }
            public int TotalCalls { get; set; }
            public int TotalOpen { get; set; }
            public int TotalClosed { get; set; }
            public int TotalOverdue { get; set; }
        }

        public class MyTeamMonth
        {
            public int Month { get; set; }
            public string MonthName { get; set; }
        }
    }
}
