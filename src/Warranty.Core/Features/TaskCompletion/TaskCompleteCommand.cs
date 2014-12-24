namespace Warranty.Core.Features.TaskCompletion
{
    using System;

    public class TaskCompleteCommand : ICommand
    {
        public Guid TaskId { get; set; }
    }
}