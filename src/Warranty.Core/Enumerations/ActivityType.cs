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
        public static readonly ActivityType Close = new ActivityType(4, "Close");
        public static readonly ActivityType Reopen = new ActivityType(5, "Reopen");
    }
}