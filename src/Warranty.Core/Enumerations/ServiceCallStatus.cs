namespace Warranty.Core.Enumerations
{
    public class ServiceCallStatus : Enumeration<ServiceCallStatus>
    {
        private ServiceCallStatus(int value, string displayName)
            :base(value, displayName)
        {
        }

        public static readonly ServiceCallStatus Requested = new ServiceCallStatus(1, "Requested");
        public static readonly ServiceCallStatus Open = new ServiceCallStatus(2, "Open");
        public static readonly ServiceCallStatus SpecialProject = new ServiceCallStatus(3, "Special Project");
        public static readonly ServiceCallStatus Closed = new ServiceCallStatus(4, "Closed");
    }
}