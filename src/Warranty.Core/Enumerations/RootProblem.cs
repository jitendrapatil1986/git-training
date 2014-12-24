namespace Warranty.Core.Enumerations
{
    public class RootProblem : Enumeration<RootProblem>
    {
        private RootProblem(int value, string displayName)
            :base(value, displayName)
        {
        }

        public static readonly RootProblem Appliances = new RootProblem(1, "Appliances");
        public static readonly RootProblem Cabinets = new RootProblem(2, "Cabinets");
        public static readonly RootProblem Countertops = new RootProblem(3, "Countertops");
        public static readonly RootProblem Decking = new RootProblem(4, "Decking");
        public static readonly RootProblem Door = new RootProblem(5, "Door");
        public static readonly RootProblem Drywall = new RootProblem(6, "Drywall");
        public static readonly RootProblem Electrical = new RootProblem(7, "Electrical");
        public static readonly RootProblem ExteriorWalls = new RootProblem(8, "Exterior Walls");
        public static readonly RootProblem Fireplace = new RootProblem(9, "Fireplace");
        public static readonly RootProblem Flatwork = new RootProblem(10, "Flatwork");
        public static readonly RootProblem Flooring = new RootProblem(11, "Flooring");
        public static readonly RootProblem Foundation = new RootProblem(12, "Foundation");
        public static readonly RootProblem HVAC = new RootProblem(13, "HVAC");
        public static readonly RootProblem Insulation = new RootProblem(14, "Insulation");
        public static readonly RootProblem Landscaping = new RootProblem(15, "Landscaping");
        public static readonly RootProblem Mechanical = new RootProblem(16, "Mechanical");
        public static readonly RootProblem Paint = new RootProblem(17, "Paint");
        public static readonly RootProblem Plumbing = new RootProblem(18, "Plumbing");
        public static readonly RootProblem Roofing = new RootProblem(19, "Roofing");
        public static readonly RootProblem Structural = new RootProblem(20, "Structural");
        public static readonly RootProblem Trim = new RootProblem(21, "Trim");
        public static readonly RootProblem Tile = new RootProblem(22, "Tile");
        public static readonly RootProblem Windows = new RootProblem(23, "Windows");
        public static readonly RootProblem Imported = new RootProblem(24, "Imported");
    }
}