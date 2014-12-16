namespace Warranty.Core.ToDoInfrastructure.Models
{
    using System;

    public class ToDoJob10MonthAnniversaryModel
    {
        public string HomeOwnerName { get; set; }
        public string AddressLine { get; set; }
        public Guid ServiceCallId { get; set; }
        public int ServiceCallNumber { get; set; }
        public Guid? JobId { get; set; }
        public int JobNumber { get; set; }
        public int YearsWithinWarranty { get; set; }
        public DateTime WarrantyStartDate { get; set; }
        public int HomeOwnerNumber { get; set; }
    }
}