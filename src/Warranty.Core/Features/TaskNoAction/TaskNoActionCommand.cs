namespace Warranty.Core.Features.TaskNoAction
{
    using System;

    public class TaskNoActionCommand : ICommand
    {
        public Guid TaskId { get; set; }
    }
}