namespace Warranty.UI.Core.Enumerations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Warranty.Core.Enumerations;

    public class NavHeaderItem : Enumeration<NavHeaderItem>
    {
        public static NavHeaderItem WsrLoadingReport = new NavHeaderItem(1, Reports.Key, Reports.Display.WsrLoadingReportName, "Report", "WSRLoadingReport");
        public static NavHeaderItem WsrBonusSummaryReport = new NavHeaderItem(2, Reports.Key, Reports.Display.WsrBonusSummaryReportName, "Report", "WarrantyBonusSummaryWSRReport");
        public static NavHeaderItem MailMergeReport = new NavHeaderItem(3, Reports.Key, Reports.Display.MailMergeReportName, "Report", "MailMerge");
        public static NavHeaderItem Achievement = new NavHeaderItem(4, Reports.Key, Reports.Display.AchievementReportName, "Report", "AchievementReport");
        public static NavHeaderItem Saltline = new NavHeaderItem(5, Reports.Key, Reports.Display.SaltlineReportName, "Report", "SaltlineReport");

        protected NavHeaderItem(int value, string key, string displayName, string controller, string action, bool hasDivider = false)
            : base(value, displayName)
        {
            Key = key;
            Controller = controller;
            Action = action;
            HasDivider = hasDivider;
        }

        public string Key { get; private set; }
        public string Controller { get; private set; }
        public string Action { get; private set; }
        public bool HasDivider { get; private set; }

        public static IEnumerable<NavHeaderItem> GetAll(string filter)
        {
            return GetAll().Where(x => string.Equals(x.Key, filter, StringComparison.InvariantCultureIgnoreCase));
        }

        public static class Reports
        {
            public const string Key = "Reports";

            internal static class Display
            {
                public const string WsrLoadingReportName = "WSR Loading Report";
                public const string MailMergeReportName = "Mail Merge Report";
                public const string WsrBonusSummaryReportName = "WSR Bonus Summary Report";
                public const string AchievementReportName = "Achievement Report";
                public const string SaltlineReportName = "Saltline Report";
            }
        }
    }
}