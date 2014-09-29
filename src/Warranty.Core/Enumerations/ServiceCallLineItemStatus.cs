namespace Warranty.Core.Enumerations
{
    public class ServiceCallLineItemStatus : Enumeration<ServiceCallLineItemStatus>
    {
        private ServiceCallLineItemStatus(int value, string displayName)
            :base(value, displayName)
        {
        }

        public static readonly ServiceCallLineItemStatus Open = new ServiceCallLineItemStatus(1, "Open");
        public static readonly ServiceCallLineItemStatus InProgress = new ServiceCallLineItemStatus(2, "In-Progress");
        public static readonly ServiceCallLineItemStatus Complete = new ServiceCallLineItemStatus(3, "Complete");
    }
}