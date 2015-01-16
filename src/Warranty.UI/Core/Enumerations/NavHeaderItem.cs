namespace Warranty.UI.Core.Enumerations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Warranty.Core.Enumerations;

    public class NavHeaderItem : Enumeration<NavHeaderItem>
    {
        public static NavHeaderItem HomePage = new NavHeaderItem(1, Home.Key, Home.Display.HomeName, "Home", "Index");

        public static NavHeaderItem WsrLoadingReport = new NavHeaderItem(2, Reports.Key, Reports.Display.WsrLoadingReportName, "Report", "WSRLoadingReport");
        public static NavHeaderItem WsrBonusSummaryReport = new NavHeaderItem(3, Reports.Key, Reports.Display.WsrBonusSummaryReportName, "Report", "WarrantyBonusSummaryWSRReport");
        public static NavHeaderItem MailMergeReport = new NavHeaderItem(4, Reports.Key, Reports.Display.MailMergeReportName, "Report", "MailMerge");
        public static NavHeaderItem Achievement = new NavHeaderItem(5, Reports.Key, Reports.Display.AchievementReportName, "Report", "AchievementReport");
        public static NavHeaderItem Saltline = new NavHeaderItem(6, Reports.Key, Reports.Display.SaltlineReportName, "Report", "SaltlineReport");
        public static NavHeaderItem WsrSummary = new NavHeaderItem(9, Reports.Key, Reports.Display.WsrSummaryReportName, "Report", "WSRSummaryReport");
        public static NavHeaderItem WsrCallSummary = new NavHeaderItem(10, Reports.Key, Reports.Display.WsrCallSummaryReportName, "Report", "WSRCallSummaryReport");
        
        public static NavHeaderItem AssignWSR = new NavHeaderItem(7, Administration.Key, Administration.Display.AssignWSRName, "Admin", "ManageAssignments");
        public static NavHeaderItem ManageCostCode = new NavHeaderItem(8, Administration.Key, Administration.Display.MarketCostCodeName, "Admin", "ManageProblemCodeCostCodes");

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

        public static class Home
        {
            public const string Key = "Home";

            internal static class Display
            {
                public const string HomeName = "Home";
            }
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
                public const string WsrSummaryReportName = "WSR Summary Report";
                public const string WsrCallSummaryReportName = "WSR Call Summary Report";
            }
        }

        public static class Administration
        {
            public const string Key = "Administration";

            internal static class Display
            {
                public const string AssignWSRName = "Assign WSRs";
                public const string MarketCostCodeName = "Market Cost Codes";
            }
        }
    }
}