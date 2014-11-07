namespace Warranty.Core.Enumerations
{
    public class BackchargeStatus: Enumeration<BackchargeStatus>
    {
        public static readonly BackchargeStatus Requested = new BackchargeStatus(1, "Requested");
        public static readonly BackchargeStatus Pending = new BackchargeStatus(2, "Pending");
        public static readonly BackchargeStatus RequestedApproval = new BackchargeStatus(3, "Requested Approval");
        public static readonly BackchargeStatus Approved = new BackchargeStatus(4, "Approved");
        public static readonly BackchargeStatus RequestedHold = new BackchargeStatus(5, "Requested Hold");
        public static readonly BackchargeStatus Hold = new BackchargeStatus(6, "Hold");
        public static readonly BackchargeStatus RequestedDeny = new BackchargeStatus(7, "Requested Deny");
        public static readonly BackchargeStatus Denied = new BackchargeStatus(8, "Denied");

        private BackchargeStatus(int value, string displayName)
            : base(value, displayName)
        {

        }
    }
}