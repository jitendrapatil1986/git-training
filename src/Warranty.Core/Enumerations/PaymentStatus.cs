namespace Warranty.Core.Enumerations
{
    using System.Linq;

    public class PaymentStatus : Enumeration<PaymentStatus>
    {
        public static readonly PaymentStatus Paid = new PaymentStatus(1, "Paid", "1");
        public static readonly PaymentStatus Pending = new PaymentStatus(2, "Pending", "P");
        public static readonly PaymentStatus Approved = new PaymentStatus(4, "Approved", "A");
        public static readonly PaymentStatus OnHold = new PaymentStatus(5, "On Hold", "H");
        public static readonly PaymentStatus NeverPay = new PaymentStatus(6, "Never Pay", "Z");
        public static readonly PaymentStatus Question = new PaymentStatus(7, "Question", "Q");
        public static readonly PaymentStatus Manual = new PaymentStatus(8, "Manual", "M");

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