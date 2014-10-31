namespace Warranty.Core.Enumerations
{
    public class PaymentStatus : Enumeration<PaymentStatus>
    {
        public PaymentStatus(int value, string displayName) : base(value, displayName)
        {
        }

        public static PaymentStatus Pending = new PaymentStatus(1, "Pending");
        public static PaymentStatus ManagerOnHold = new PaymentStatus(2, "Manager On Hold");
        public static PaymentStatus ManagerDenied = new PaymentStatus(3, "Manager Denied");
        public static PaymentStatus ManagerApproved = new PaymentStatus(4, "Manager Approved");
        public static PaymentStatus AccountingOnHold = new PaymentStatus(5, "Accounting On Hold");
        public static PaymentStatus AccountingDenied = new PaymentStatus(6, "Accounting Denied");
        public static PaymentStatus AccountingApproved = new PaymentStatus(7, "Accounting Approved");
        public static PaymentStatus Paid = new PaymentStatus(8, "Paid");
    }
}