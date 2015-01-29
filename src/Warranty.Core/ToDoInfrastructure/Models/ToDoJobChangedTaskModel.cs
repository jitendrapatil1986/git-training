namespace Warranty.Core.ToDoInfrastructure.Models
{
    using System;
    using Enumerations;

    public class ToDoJobChangedTaskModel
    {
        public Guid TaskId { get; set; }
        public TaskType TaskType { get; set; }
        public string Description { get; set; }
        public string Html { get; set; }
        public Guid JobId { get; set; }
        public string JobNumber { get; set; }
    }
}