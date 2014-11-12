namespace Warranty.Core.Enumerations
{
    public class ActivityType : Enumeration<ActivityType>
    {
        private ActivityType(int value, string displayName)
            : base(value, displayName)
        {
        }

        public static readonly ActivityType Reassignment = new ActivityType(1, "Reassignment");
        public static readonly ActivityType SpecialProject = new ActivityType(2, "Special Project");
        public static readonly ActivityType Escalation = new ActivityType(3, "Escalation");
        public static readonly ActivityType ChangePhone = new ActivityType(4, "Change Phone");
        public static readonly ActivityType ChangeEmail = new ActivityType(5, "Change Email");
        public static readonly ActivityType Complete = new ActivityType(6, "Complete");
        public static readonly ActivityType Reopen = new ActivityType(7, "Reopen");
        public static readonly ActivityType UploadAttachment = new ActivityType(8, "Upload Attachment");
        public static readonly ActivityType DeletedAttachment = new ActivityType(9, "Deleted Attachment");
        public static readonly ActivityType RenamedAttachment = new ActivityType(10, "Renamed Attachment");
        public static readonly ActivityType PaymentOnHold = new ActivityType(11, "Payment On Hold");
        public static readonly ActivityType PaymentApprove = new ActivityType(12, "Payment Approval");
        public static readonly ActivityType PaymentDelete = new ActivityType(13, "Payment Delete");
    }
}