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
        public static readonly TaskType Job10MonthAnniversary = new TaskType(6, "Job 10 Month Anniversary - 1 Year Walk");
        public static readonly TaskType PaymentStatusChanged = new TaskType(7, "Payment Status Changed");
        public static readonly TaskType JobStage3 = new TaskType(8, "Job Stage 3 - Warranty Introduction");
        public static readonly TaskType JobStage7 = new TaskType(9, "Job Stage 7 - 244 Walk");
        public static readonly TaskType JobStage9 = new TaskType(10, "Job Stage 9 - Warranty Orientation");
    }
}