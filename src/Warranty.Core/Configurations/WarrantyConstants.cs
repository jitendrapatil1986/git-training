﻿namespace Warranty.Core.Configurations
{
    using System.Collections.Generic;

    public class WarrantyConstants
    {
        public const int NumberOfDaysAllowedToCloseServiceCall = 7;
        public const decimal WarrantySpentGoal = 40;
        public const int NumberOfYearsHomeIsWarrantable = 10;
        public const string AttachmentRemovalMessage = "Are you sure you want to remove this attachment?";

        public static readonly List<string> LaborObjectAccounts = new List<string> { "9425", "9435" };
        public static readonly List<string> MaterialObjectAccounts = new List<string> { "9430", "9440" };

        public static readonly string UnderTwoYearLaborCode = LaborObjectAccounts[0];
        public static readonly string OverTwoYearLaborCode = LaborObjectAccounts[1];

        public static readonly string UnderTwoYearMaterialCode = MaterialObjectAccounts[0];
        public static readonly string OverTwoYearMaterialCode = MaterialObjectAccounts[1];

    }
}
