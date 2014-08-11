namespace Warranty.Core.Enumerations
{
    public class WarrantyCallStatus : Enumeration<WarrantyCallStatus>
    {
        private WarrantyCallStatus(int value, string displayName)
            :base(value, displayName)
        {
        }

        public static readonly WarrantyCallStatus Requested = new WarrantyCallStatus(1, "Requested");
        public static readonly WarrantyCallStatus Open = new WarrantyCallStatus(2, "Open");
        public static readonly WarrantyCallStatus SpecialProject = new WarrantyCallStatus(3, "Special Project");
        public static readonly WarrantyCallStatus Closed = new WarrantyCallStatus(4, "Closed");
    }
}