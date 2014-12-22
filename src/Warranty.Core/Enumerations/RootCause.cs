namespace Warranty.Core.Enumerations
{
    public class RootCause : Enumeration<RootCause>
    {
        private RootCause(int value, string displayName)
            :base(value, displayName)
        {
        }

        public static readonly RootCause BuilderIncomplete = new RootCause(1, "Builder Incomplete");
        public static readonly RootCause CustomerDelight = new RootCause(2, "Customer Delight");
        public static readonly RootCause NaturalCauses = new RootCause(3, "Natural Causes");
        public static readonly RootCause ProductDefect = new RootCause(4, "Product Defect");
        public static readonly RootCause WorkmanshipInstall = new RootCause(5, "Workmanship Install");
    }
}