namespace Warranty.Core.Enumerations
{
    using System.Linq;

    public class PaymentStatus : Enumeration<PaymentStatus>
    {
        public static readonly PaymentStatus Requested = new PaymentStatus(1, "Requested", "NoJdeCode-WarrantyModuleStatus");
        public static readonly PaymentStatus Pending = new PaymentStatus(2, "Pending", "P");
        public static readonly PaymentStatus RequestedApproval = new PaymentStatus(3, "Requested Approval", "NoJdeCode-WarrantyModuleStatus");
        public static readonly PaymentStatus Approved = new PaymentStatus(4, "Approved", "A");
        public static readonly PaymentStatus RequestedHold = new PaymentStatus(5, "Requested Hold", "NoJdeCode-WarrantyModuleStatus");
        public static readonly PaymentStatus Hold = new PaymentStatus(6, "Hold", "H");
        public static readonly PaymentStatus NeverPay = new PaymentStatus(7, "Never Pay", "Z");
        public static readonly PaymentStatus Question = new PaymentStatus(8, "Question", "Q");
        public static readonly PaymentStatus Manual = new PaymentStatus(9, "Manual", "M");
        public static readonly PaymentStatus Paid = new PaymentStatus(10, "Paid", "1");
        public static readonly PaymentStatus RequestedDeny = new PaymentStatus(11, "Requested Deny", "NoJdeCode-WarrantyModuleStatus");

        private PaymentStatus(int value, string displayName, string jdeCode)
            : base(value, displayName)
        {
            JdeCode = jdeCode;
        }

        public string JdeCode { get; set; }

        public static PaymentStatus FromJdeCode(string code)
        {
            return GetAll().SingleOrDefault(x => x.JdeCode.Equals(code));
        }
    }
}