namespace Warranty.Core.Features.TaskNoAction
{
    using Entities;
    using NPoco;

    public class TaskNoActionCommandHandler : ICommandHandler<TaskNoActionCommand>
    {
        private readonly IDatabase _database;

        public TaskNoActionCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(TaskNoActionCommand message)
        {
            using (_database)
            {
                var task = _database.SingleOrDefaultById<Task>(message.TaskId);
                if (task != null)
                {
                    task.IsNoAction = true;
                    _database.Update(task);
                }
            }
        }
    }
}