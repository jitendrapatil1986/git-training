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
    }
}