namespace Warranty.UI.Core.Enumerations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Warranty.Core.Enumerations;

    public class NavHeaderItem : Enumeration<NavHeaderItem>
    {
        public static NavHeaderItem CreatePanelCharge = new NavHeaderItem(1, Create.Key, Create.Display.ServiceCall, "ServiceCall", "SearchCustomer");

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

        public static class Create
        {
            public const string Key = "Create";

            internal static class Display
            {
                public const string ServiceCall = "Service Call";
            }
        }
    }
}