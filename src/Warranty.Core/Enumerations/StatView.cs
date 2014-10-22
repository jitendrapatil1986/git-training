namespace Warranty.Core.Enumerations
{
    public class StatView : Yay.Enumerations.Enumeration<StatView>
    {
        public static readonly StatView DollarsSpent = new StatView(1, "Spent", "DollarsSpentPerHome", "DESC");
        public static readonly StatView AvgDaysClosed = new StatView(2, "AvgDays", "AverageDaysClosed", "DESC");
        public static readonly StatView PercentClosed = new StatView(3, "PercentClosed", "PercentClosedWithinSevenDays", "ASC");
        public static readonly StatView PercentDefinitelyRecommend = new StatView(4, "PercentRecommend", "PercentRecommend", "ASC");

        private StatView(int value, string displayName, string orderByColumnName, string sortOrder) : base(value, displayName)
        {
            OrderByColumnName = orderByColumnName;
            SortOrder = sortOrder;
        }

        public string OrderByColumnName { get; set; }
        public string SortOrder { get; set; }

        public string Highlight(StatView view)
        {
            if (this == view)
                return "warning";

            return "hidden-xs";
        }

        public string ShowChart(StatView view)
        {
            if (this == view)
                return "";

            return "hidden";
        }

        public string Active(StatView view)
        {
            if (this == view)
                return "active";

            return "";
        }
    }
}
