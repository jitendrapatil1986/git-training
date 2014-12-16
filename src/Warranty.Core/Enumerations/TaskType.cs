namespace Warranty.Core.Enumerations
{
    public class TaskType : Enumeration<TaskType>
    {
        private TaskType(int value, string displayName)
            : base(value, displayName)
        {
        }

        public static readonly TaskType JobStageChanged = new TaskType(1, "Job Stage Changed");
        public static readonly TaskType JobClosed = new TaskType(2, "Job Closed");
        public static readonly TaskType Job3MonthAnniversary = new TaskType(3, "Job 3 Month Anniversary");
        public static readonly TaskType Job5MonthAnniversary = new TaskType(4, "Job 5 Month Anniversary");
        public static readonly TaskType Job9MonthAnniversary = new TaskType(5, "Job 9 Month Anniversary");
        public static readonly TaskType Job10MonthAnniversary = new TaskType(6, "Job 10 Month Anniversary");
    }
}