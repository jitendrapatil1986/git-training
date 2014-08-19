namespace Warranty.Core.Enumerations
{
    public class Month : Enumeration<Month>
    {
        public static readonly Month January = new Month(1, "January", "Jan", 1);
        public static readonly Month February = new Month(2, "February", "Feb", 1);
        public static readonly Month March = new Month(3, "March", "Mar", 1);
        public static readonly Month April = new Month(4, "April", "Apr", 2);
        public static readonly Month May = new Month(5, "May", "May", 2);
        public static readonly Month June = new Month(6, "June", "Jun", 2);
        public static readonly Month July = new Month(7, "July", "Jul", 3);
        public static readonly Month August = new Month(8, "August", "Aug", 3);
        public static readonly Month September = new Month(9, "September", "Sep", 3);
        public static readonly Month October = new Month(10, "October", "Oct", 4);
        public static readonly Month November = new Month(11, "November", "Nov", 4);
        public static readonly Month December = new Month(12, "December", "Dec", 4);

        private Month(int value, string displayName, string abbreviation, int quarter)
            : base(value, displayName)
        {
            Abbreviation = abbreviation;
            Quarter = quarter;
        }

        public string Abbreviation { get; set; }
        public int Quarter { get; set; }
    }
}