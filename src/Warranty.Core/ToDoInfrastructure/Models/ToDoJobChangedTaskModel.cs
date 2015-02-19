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
        public string HomeownerName { get; set; }
        public string HomePhone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string StateCode { get; set; }
        public string PostalCode { get; set; }
    }
}