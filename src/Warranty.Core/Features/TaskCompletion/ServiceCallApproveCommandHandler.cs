namespace Warranty.Core.Features.TaskCompletion
{
    using Entities;
    using NPoco;

    public class TaskCompleteCommandHandler : ICommandHandler<TaskCompleteCommand>
    {
        private readonly IDatabase _database;

        public TaskCompleteCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(TaskCompleteCommand message)
        {
            using (_database)
            {
                var task = _database.SingleOrDefaultById<Task>(message.TaskId);
                if (task != null)
                {
                    task.IsComplete = true;
                    _database.Update(task);
                }
            }
        }
    }
}
