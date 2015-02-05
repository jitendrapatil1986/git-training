namespace Warranty.Core.ToDoInfrastructure.Models
{
    using System;

    public class ToDoJobChangedTaskApprovalModel
    {
        public Guid TaskId { get; set; }
        public string Description { get; set; }
        public Guid JobId { get; set; }
        public string JobNumber { get; set; } 
    }
}