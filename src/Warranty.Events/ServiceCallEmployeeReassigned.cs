namespace Warranty.Events
{
    using NServiceBus;

    public class ServiceCallEmployeeReassigned : IEvent
    {
        public int ServiceCallNumber { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
    }
}