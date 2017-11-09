namespace Warranty.Core.Enumerations
{
    using System.Collections.Generic;
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

        public static readonly TaskType Job10MonthAnniversary = new TaskType(6, "Job 10 Month Anniversary - 1 Year Walk", true, null, "1 Year Walkthrough");
        public static readonly TaskType PaymentStatusChanged = new TaskType(7, "Payment Status Changed", false);
        public static readonly TaskType QualityIntroductionOfWSR = new TaskType(8, "Job is at Stage 1 - time to schedule a Quality Introduction of WSR.", true, 1, "Quality Introduction of WSR");
        public static readonly TaskType WarrantyWalk = new TaskType(9, "Job is at Stage 7 - time to schedule a warranty walk.", true, 7, "Warranty Walk");
        public static readonly TaskType JobStage10JobClosed = new TaskType(10, "Job Closed - Warranty Orientation Due", true, 10, "Warranty Orientation");
        public static readonly TaskType JobStage10Approval = new TaskType(11, "Warranty Orientation Approval", true);

        private TaskType(int value, string displayName, bool isTransferable, int? stage = null, string reportDisplay = "")
            : base(value, displayName)
        {
            IsTransferable = isTransferable;
            Stage = stage;
            ReportDisplay = reportDisplay;
        }

        public bool IsTransferable { get; private set; }

        public int? Stage { get; private set; }

        public string ReportDisplay { get; private set; }

        public bool ShowInReports { get { return !string.IsNullOrWhiteSpace(ReportDisplay); } }

        public static IEnumerable<TaskType> ValidModelOrShowcaseTasks
        {
            get
            {
                return new List<TaskType>
                {
                    WarrantyWalk
                };
            }
        }

        public static IEnumerable<TaskType> ValidSoldJobTasks
        {
            get
            {
                return new List<TaskType>
                {
                    QualityIntroductionOfWSR,
                    WarrantyWalk
                };
            }
        }
    }
}