namespace Warranty.Server.Configuration
{
    using System.Collections.Generic;

    public class WarrantyConstants
    {
        public static readonly string ProgramId = "WARRANTY";
        public static readonly string PaymentType = "EO";
        public static readonly string VarianceCode = "W";

        public static readonly List<string> LaborObjectAccounts = new List<string> { "9425", "9435" };
        public static readonly List<string> MaterialObjectAccounts = new List<string> {"9430", "9440"};

        public static readonly string UnderTwoYearLaborCode = LaborObjectAccounts[0];
        public static readonly string OverTwoYearLaborCode = LaborObjectAccounts[1];

        public static readonly string UnderTwoYearMaterialCode = MaterialObjectAccounts[0];
        public static readonly string OverTwoYearMaterialCode = MaterialObjectAccounts[1];

    }
}
