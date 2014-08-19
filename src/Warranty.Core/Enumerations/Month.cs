namespace Warranty.Core.Enumerations
{
    public class Month : Enumeration<Month>
    {
        public static readonly Month January = new Month(1, "January", "Jan");
        public static readonly Month February = new Month(2, "February", "Feb");
        public static readonly Month March = new Month(3, "March", "Mar");
        public static readonly Month April = new Month(4, "April", "Apr");
        public static readonly Month May = new Month(5, "May", "May");
        public static readonly Month June = new Month(6, "June", "Jun");
        public static readonly Month July = new Month(7, "July", "Jul");
        public static readonly Month August = new Month(8, "August", "Aug");
        public static readonly Month September = new Month(9, "September", "Sep");
        public static readonly Month October = new Month(10, "October", "Oct");
        public static readonly Month November = new Month(11, "November", "Nov");
        public static readonly Month December = new Month(12, "December", "Dec");

        private Month(int value, string displayName, string abbreviation)
            : base(value, displayName)
        {
            Abbreviation = abbreviation;
        }

        public string Abbreviation { get; set; }
    }
}