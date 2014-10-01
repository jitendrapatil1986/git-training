namespace Warranty.Core.Enumerations
{
    public class ReferenceType : Enumeration<ReferenceType>
    {
        private ReferenceType(int value, string displayName)
            : base(value, displayName)
        {
        }

        public static readonly ReferenceType LineItem = new ReferenceType(1, "Line Item");
        public static readonly ReferenceType ServiceCall = new ReferenceType(2, "Service Call");
        public static readonly ReferenceType Job = new ReferenceType(3, "Job");
        public static readonly ReferenceType Homeowner = new ReferenceType(4, "Homeowner");
    }
}