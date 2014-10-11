﻿namespace Warranty.UI.Core.Enumerations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Warranty.Core.Enumerations;

    public class NavHeaderItem : Enumeration<NavHeaderItem>
    {
        public static NavHeaderItem WsrLoadingReport = new NavHeaderItem(1, Reports.Key, Reports.Display.WsrLoadingReportName, "Report", "WSRLoadingReport");
        public static NavHeaderItem MailMergeReport = new NavHeaderItem(2, Reports.Key, Reports.Display.MailMergeReportName, "Report", "MailMerge");

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
            }
        }
    }
}