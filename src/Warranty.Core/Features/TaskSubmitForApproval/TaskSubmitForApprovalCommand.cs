namespace Warranty.Core.Features.TaskSubmitForApproval
{
    using System;

    public class TaskSubmitForApprovalCommand : ICommand
    {
        public Guid TaskId { get; set; }
    }
}