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
        public static readonly ServiceCallStatus Closed = new ServiceCallStatus(3, "Closed");
        public static readonly ServiceCallStatus HomeownerSigned = new ServiceCallStatus(4, "Homeowner Signed");
    }
}