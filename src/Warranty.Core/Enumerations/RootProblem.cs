namespace Warranty.Core.Enumerations
{
    using Yay.Enumerations;

    public class RootProblem : Yay.Enumerations.Enumeration<RootProblem>
    {
        public WarrantyCostCode CostCode { get; set; }

        private RootProblem(int value, string displayName, WarrantyCostCode costCode)
            :base(value, displayName)
        {
            CostCode = costCode;
        }

        public static readonly RootProblem Appliances = new RootProblem(1, "Appliances", WarrantyCostCode.InteriorMisc);
        public static readonly RootProblem Cabinets = new RootProblem(2, "Cabinets", WarrantyCostCode.Cabinets);
        public static readonly RootProblem Countertops = new RootProblem(3, "Countertops", WarrantyCostCode.InteriorMisc);
        public static readonly RootProblem Decking = new RootProblem(4, "Decking", WarrantyCostCode.ExteriorMisc);
        public static readonly RootProblem Door = new RootProblem(5, "Door", WarrantyCostCode.Weatherstrip);
        public static readonly RootProblem Drywall = new RootProblem(6, "Drywall", WarrantyCostCode.Sheetrock);
        public static readonly RootProblem Electrical = new RootProblem(7, "Electrical", WarrantyCostCode.Electrical);
        public static readonly RootProblem ExteriorWalls = new RootProblem(8, "Exterior Walls", WarrantyCostCode.ExteriorMisc);
        public static readonly RootProblem Fireplace = new RootProblem(9, "Fireplace", WarrantyCostCode.Fireplaces);
        public static readonly RootProblem Flatwork = new RootProblem(10, "Flatwork", WarrantyCostCode.ConcreteFlatwork);
        [Deprecated]
        public static readonly RootProblem Flooring = new RootProblem(11, "Flooring", WarrantyCostCode.AllFlooring);
        public static readonly RootProblem Foundation = new RootProblem(12, "Foundation", WarrantyCostCode.Structural);
        public static readonly RootProblem HVAC = new RootProblem(13, "HVAC", WarrantyCostCode.Hvac);
        public static readonly RootProblem Insulation = new RootProblem(14, "Insulation", WarrantyCostCode.InteriorMisc);
        public static readonly RootProblem Landscaping = new RootProblem(15, "Landscaping", WarrantyCostCode.LscapeFenceTrees);
        public static readonly RootProblem Mechanical = new RootProblem(16, "Mechanical", WarrantyCostCode.InteriorMisc);
        public static readonly RootProblem Paint = new RootProblem(17, "Paint", WarrantyCostCode.AllPaint);
        public static readonly RootProblem Plumbing = new RootProblem(18, "Plumbing", WarrantyCostCode.Plumbing);
        public static readonly RootProblem Roofing = new RootProblem(19, "Roofing", WarrantyCostCode.RoofFlashing);
        public static readonly RootProblem Structural = new RootProblem(20, "Structural", WarrantyCostCode.Structural);
        public static readonly RootProblem Trim = new RootProblem(21, "Trim", WarrantyCostCode.TrimAllInterior);
        public static readonly RootProblem Tile = new RootProblem(22, "Tile", WarrantyCostCode.InteriorMisc);
        public static readonly RootProblem Windows = new RootProblem(23, "Windows", WarrantyCostCode.Windows);
        [Deprecated]
        public static readonly RootProblem Imported = new RootProblem(24, "Imported", WarrantyCostCode.DoNotUse);
        public static readonly RootProblem Hardwood = new RootProblem(25, "Hardwood", WarrantyCostCode.AllFlooring);
        public static readonly RootProblem Carpet = new RootProblem(26, "Carpet", WarrantyCostCode.AllFlooring);
        public static readonly RootProblem Brick = new RootProblem(27, "Brick", WarrantyCostCode.Brick);
        public static readonly RootProblem CustomerDelight = new RootProblem(28, "Customer Delight", WarrantyCostCode.AllCleaning);
        public static readonly RootProblem Mirror = new RootProblem(29, "Mirror", WarrantyCostCode.InteriorMisc);
        public static readonly RootProblem SidingCornice = new RootProblem(30, "Siding/Cornice", WarrantyCostCode.ExteriorMisc);
        public static readonly RootProblem TubShower = new RootProblem(31, "Tub/Shower", WarrantyCostCode.BathsMarbleTubs);
        public static readonly RootProblem Garage = new RootProblem(32, "Garage", WarrantyCostCode.Weatherstrip);
        public static readonly RootProblem FinalGrade = new RootProblem(33, "Final Grade", WarrantyCostCode.ExteriorMisc);
        public static readonly RootProblem Fence = new RootProblem(34, "Fence", WarrantyCostCode.LscapeFenceTrees);
        public static readonly RootProblem Frame = new RootProblem(35, "Frame", WarrantyCostCode.FrameCornice);
        public static readonly RootProblem Sprinkler = new RootProblem(36, "Sprinkler", WarrantyCostCode.LscapeFenceTrees);
        public static readonly RootProblem Gutter = new RootProblem(37, "Gutter", WarrantyCostCode.ExteriorMisc);
        public static readonly RootProblem HardwareFixture = new RootProblem(38, "Hardware/Fixture", WarrantyCostCode.FixturesHardware);
        public static readonly RootProblem NoAction = new RootProblem(39, "No Action", WarrantyCostCode.AllCleaning);
        public static readonly RootProblem Septic = new RootProblem(40, "Septic", WarrantyCostCode.Plumbing);
    }
}
