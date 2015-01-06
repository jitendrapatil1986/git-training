namespace Warranty.Core.Enumerations
{
    using Yay.Enumerations;

    public class WarrantyCostCode : Enumeration<WarrantyCostCode>
    {
        public static readonly WarrantyCostCode ConcreteFlatwork = new WarrantyCostCode(1, "Concrete, Flatwork", "01");
        public static readonly WarrantyCostCode FrameCornice = new WarrantyCostCode(2, "Frame, Cornice", "02");
        public static readonly WarrantyCostCode RoofFlashing = new WarrantyCostCode(3, "Roof, Flashing", "03");
        public static readonly WarrantyCostCode Sheetrock = new WarrantyCostCode(4, "Sheetrock", "04");
        public static readonly WarrantyCostCode Brick = new WarrantyCostCode(5, "Brick", "05");
        public static readonly WarrantyCostCode AllPaint = new WarrantyCostCode(6, "All Paint", "06");
        public static readonly WarrantyCostCode TrimAllInterior = new WarrantyCostCode(7, "Trim (All Interior)", "07");
        public static readonly WarrantyCostCode BathsMarbleTubs = new WarrantyCostCode(8, "Baths, Marble, Tubs ", "08");
        public static readonly WarrantyCostCode Weatherstrip = new WarrantyCostCode(9, "Weatherstrip", "09");
        public static readonly WarrantyCostCode Plumbing = new WarrantyCostCode(10, "Plumbing ", "10");
        public static readonly WarrantyCostCode Hvac = new WarrantyCostCode(11, "HVAC", "11");
        public static readonly WarrantyCostCode AllCleaning = new WarrantyCostCode(12, "All Cleaning", "12");
        public static readonly WarrantyCostCode AllFlooring = new WarrantyCostCode(13, "All Flooring", "13");
        public static readonly WarrantyCostCode Wallpaper = new WarrantyCostCode(14, "Wallpaper", "14");
        public static readonly WarrantyCostCode LscapeFenceTrees = new WarrantyCostCode(15, "Lscape, Fence, Trees", "15");
        public static readonly WarrantyCostCode DrainageFillDirt = new WarrantyCostCode(16, "Drainage, Fill Dirt", "16");
        public static readonly WarrantyCostCode FixturesHardware = new WarrantyCostCode(17, "Fixtures, Hardware", "17");
        public static readonly WarrantyCostCode ReglazesTubChips = new WarrantyCostCode(18, "Reglazes, Tub Chips", "18");
        public static readonly WarrantyCostCode InteriorMisc = new WarrantyCostCode(19, "Interior Misc.", "19");
        public static readonly WarrantyCostCode ExteriorMisc = new WarrantyCostCode(20, "Exterior Misc.", "20");
        public static readonly WarrantyCostCode Electrical = new WarrantyCostCode(21, "Electrical", "21");
        public static readonly WarrantyCostCode Windows = new WarrantyCostCode(22, "Windows", "22");
        public static readonly WarrantyCostCode Cabinets = new WarrantyCostCode(23, "Cabinets ", "23");
        public static readonly WarrantyCostCode Fireplaces = new WarrantyCostCode(24, "Fireplaces ", "24");
        public static readonly WarrantyCostCode Structural = new WarrantyCostCode(25, "Structural ", "25");
        public static readonly WarrantyCostCode OneyrTouchUp = new WarrantyCostCode(26, "1 yr Touch Up ", "30");
        public static readonly WarrantyCostCode MaterialDiscounts = new WarrantyCostCode(27, "Material Discounts", "50");
        [Deprecated]
        public static readonly WarrantyCostCode DoNotUse = new WarrantyCostCode(28, "DO NOT USE", "");


        private WarrantyCostCode(int value, string displayName, string costCode)
            : base(value, displayName)
        {
            CostCode = costCode;
        }

        public string CostCode { get; set; }
    }
}