namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

    public class TaskMap : AuditableEntityMap<Task>
    {
        public TaskMap()
        {
            TableName("Tasks")
                .PrimaryKey("TaskId", false)
                .Columns(x =>
                    {
                        x.Column(y => y.EmployeeId);
                        x.Column(y => y.Description);
                        x.Column(y => y.IsComplete);
                        x.Column(y => y.ReferenceId);
                        x.Column(y => y.TaskType);
                    });
        }
    }
}