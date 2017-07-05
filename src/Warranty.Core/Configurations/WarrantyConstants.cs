namespace Warranty.Core.Configurations
{
    using System.Collections.Generic;

    public class WarrantyConstants
    {
        public const int NumberOfDaysAllowedToCloseServiceCall = 7;
        public const decimal WarrantySpentGoal = 40;
        public const int NumberOfYearsHomeIsWarrantable = 10;
        public const string AttachmentRemovalMessage = "Are you sure you want to remove this attachment?";
        public const string DefaultActiveCommunityCode = "A";
        public const int DefaultWidgetSize = 5;
        public static readonly int DefaultJdePurchaseOrderLineItemDescriptionLength = 30;
        public static readonly int DefaultJdePurchaseOrderNotesLength = 60;
        
        //personNotified
        public static readonly int maxPersonNotifiedLength = 40;
        //invoiceNumber
        public static readonly int maxInvoiceNumberLength = 25;
        public static readonly List<string> LaborObjectAccounts = new List<string> { "9430", "9440" };
        public static readonly List<string> MaterialObjectAccounts = new List<string> { "9425", "9435" };

        public static readonly string UnderOneYearLaborCode = LaborObjectAccounts[0];
        public static readonly string OverOneYearLaborCode = LaborObjectAccounts[1];

        public static readonly string UnderOneYearMaterialCode = MaterialObjectAccounts[0];
        public static readonly string OverOneYearMaterialCode = MaterialObjectAccounts[1];

    }
}
