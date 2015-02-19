namespace Warranty.Core.Enumerations
{
    using Yay.Enumerations;

    public class TaskType : Enumeration<TaskType>
    {
        private TaskType(int value, string displayName)
            : base(value, displayName)
        {
        }
        
        [Deprecated]
        public static readonly TaskType JobStageChanged = new TaskType(1, "Job Stage Changed");
        [Deprecated]
        public static readonly TaskType JobClosed = new TaskType(2, "Job Closed");
        [Deprecated]
        public static readonly TaskType Job3MonthAnniversary = new TaskType(3, "Job 3 Month Anniversary");
        [Deprecated]
        public static readonly TaskType Job5MonthAnniversary = new TaskType(4, "Job 5 Month Anniversary");
        [Deprecated]
        public static readonly TaskType Job9MonthAnniversary = new TaskType(5, "Job 9 Month Anniversary");

        public static readonly TaskType Job10MonthAnniversary = new TaskType(6, "Job 10 Month Anniversary - 1 Year Walk");
        public static readonly TaskType PaymentStatusChanged = new TaskType(7, "Payment Status Changed");
        public static readonly TaskType JobStage3 = new TaskType(8, "Job Stage 3 - Warranty Introduction");
        public static readonly TaskType JobStage7 = new TaskType(9, "Job Stage 7 - 244 Walk");
        public static readonly TaskType JobStage10 = new TaskType(10, "Job Stage 10 - Warranty Orientation");
        public static readonly TaskType JobStage10Approval = new TaskType(11, "Job Stage 10 - Warranty Orientation Approval");
    }
}