﻿namespace Warranty.Core.ToDoInfrastructure.Models
{
    public class ToDoServiceCallRequestApprovalModel
    {
        public string HomeOwnerName { get; set; }
        public string HomeOwnerAddress { get; set; }
        public int  YearsWithinWarranty { get; set; }
    }
}