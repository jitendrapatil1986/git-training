namespace Warranty.Core.Enumerations
{
    public class WarrantyCallStatus : Enumeration<WarrantyCallStatus>
    {
        private WarrantyCallStatus(int value, string displayName)
            :base(value, displayName)
        {
        }

        public static readonly WarrantyCallStatus BuilderIncomplete = new WarrantyCallStatus(1, "Requested");
        public static readonly WarrantyCallStatus CustomerGoodwill = new WarrantyCallStatus(2, "Open");
        public static readonly WarrantyCallStatus Warrantable = new WarrantyCallStatus(3, "Special Project");
        public static readonly WarrantyCallStatus NoAction = new WarrantyCallStatus(4, "Closed");
    }
}