namespace Warranty.Core.Features.TaskSubmitForApproval
{
    using Entities;
    using Enumerations;
    using NPoco;

    public class TaskSubmitForApprovalCommandHandler : ICommandHandler<TaskSubmitForApprovalCommand>
    {
        private readonly IDatabase _database;

        public TaskSubmitForApprovalCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(TaskSubmitForApprovalCommand message)
        {
            using (_database)
            {
                var task = _database.SingleOrDefaultById<Task>(message.TaskId);
                if (task != null)
                {
                    task.IsComplete = true;
                    _database.Update(task);

                    var jobStage9ApprovalTask = new Task
                        {
                            EmployeeId = task.EmployeeId,
                            TaskType = TaskType.JobStage9Approval,
                            Description = TaskType.JobStage9Approval.DisplayName,
                            ReferenceId = task.ReferenceId,
                        };
                    _database.Insert(jobStage9ApprovalTask);
                }
            }
        }
    }
}