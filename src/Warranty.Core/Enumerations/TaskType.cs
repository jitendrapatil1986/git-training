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
    }
}