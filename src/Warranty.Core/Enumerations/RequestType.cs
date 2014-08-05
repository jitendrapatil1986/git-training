namespace Warranty.Core.Enumerations
{
    public class RequestType : Enumeration<RequestType>
    {
        private RequestType(int value, string displayName) 
            : base(value, displayName)
        {
        }

        public static readonly RequestType WarrantyRequest = new RequestType(1, "Warranty Request");
    }
}