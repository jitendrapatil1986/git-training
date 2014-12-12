﻿namespace Warranty.Server.Configuration
{
    using System.Collections.Generic;

    public class WarrantyConstants
    {
        public static readonly string ProgramId = "WARRANTY";
        public static readonly string PaymentType = "E";
        public static readonly string VarianceCode = "W";
        public static readonly string DefaultPurchaseOrderType = "EO";
        public static readonly string DefaultWarrantyJobNumberSuffix = "9999";

        public static readonly List<string> LaborObjectAccounts = new List<string> { "9425", "9435" };
        public static readonly List<string> MaterialObjectAccounts = new List<string> {"9430", "9440"};
    }
}
