namespace Warranty.Core.Enumerations
{
    public class ActivityType : Enumeration<ActivityType>
    {
        private ActivityType(int value, string displayName)
            : base(value, displayName)
        {
        }

        public static readonly ActivityType Reassignment = new ActivityType(1, "Reassignment");
        public static readonly ActivityType MarkAsSpecialProject = new ActivityType(2, "Mark As Special Project");
        public static readonly ActivityType UnMarkAsSpecialProject = new ActivityType(3, "Unmark As Special Project");
        public static readonly ActivityType Escalate = new ActivityType(4, "Escalate");
        public static readonly ActivityType Deescalate = new ActivityType(5, "Deescalate");
    }
}