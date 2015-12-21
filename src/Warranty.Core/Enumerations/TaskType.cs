namespace Warranty.Core.Enumerations
{
    using Yay.Enumerations;

    public class TaskType : Enumeration<TaskType>
    {
        

        [Deprecated]
        public static readonly TaskType JobStageChanged = new TaskType(1, "Job Stage Changed", false);
        [Deprecated]
        public static readonly TaskType JobClosed = new TaskType(2, "Job Closed", false);
        [Deprecated]
        public static readonly TaskType Job3MonthAnniversary = new TaskType(3, "Job 3 Month Anniversary", true);
        [Deprecated]
        public static readonly TaskType Job5MonthAnniversary = new TaskType(4, "Job 5 Month Anniversary", true);
        [Deprecated]
        public static readonly TaskType Job9MonthAnniversary = new TaskType(5, "Job 9 Month Anniversary", true);

        public static readonly TaskType Job10MonthAnniversary = new TaskType(6, "Job 10 Month Anniversary - 1 Year Walk", true);
        public static readonly TaskType PaymentStatusChanged = new TaskType(7, "Payment Status Changed", false);
        public static readonly TaskType JobStage3 = new TaskType(8, "Job is at Stage 3 - time to schedule a warranty introduction.", true);
        public static readonly TaskType JobStage7 = new TaskType(9, "Job is at Stage 7 - time to schedule a 244 walk.", true);
        public static readonly TaskType JobStage10 = new TaskType(10, "Job is at Stage 10 - time to schedule a warranty orientation.", true);
        public static readonly TaskType JobStage10Approval = new TaskType(11, "Job Stage 10 - Warranty Orientation Approval", true);

        private TaskType(int value, string displayName, bool isTransferable)
            : base(value, displayName)
        {
            IsTransferable = isTransferable;
        }

        public bool IsTransferable { get; private set; }
    }
}