namespace Warranty.Core.Enumerations
{
    using System.Linq;

    public class DeliveryInstruction : Enumeration<DeliveryInstruction>
    {
        private DeliveryInstruction(int value, string displayName, string jdeCode)
            : base(value, displayName)
        {
            JdeCode = jdeCode;
        }

        public string JdeCode { get; set; }

        public static DeliveryInstruction FromJdeCode(string code)
        {
            return GetAll().SingleOrDefault(x => x.JdeCode.Equals(code));
        }

        public static readonly DeliveryInstruction ASAP = new DeliveryInstruction(1, "ASAP", "ASAP");
        public static readonly DeliveryInstruction Other = new DeliveryInstruction(2, "See Notes", "BELOW");
        public static readonly DeliveryInstruction AMDelivery = new DeliveryInstruction(3, "AM Delivery", "AMDELIV");
        public static readonly DeliveryInstruction CallBeforeDelivery = new DeliveryInstruction(4, "Call Before Delivery", "CALL");
        public static readonly DeliveryInstruction CallToConfirm = new DeliveryInstruction(5, "Call to Confirm", "CALLWSR");
        public static readonly DeliveryInstruction DeliverBeforeNoon = new DeliveryInstruction(6, "Deliver Before Noon", "MORNING");
        public static readonly DeliveryInstruction DeliverToConstructionTrailer = new DeliveryInstruction(7, "Deliver to Construction Trailer", "TRAILER");
        public static readonly DeliveryInstruction DeliverToModelHome = new DeliveryInstruction(8, "Deliver to Model Home", "MODELHOME");
        public static readonly DeliveryInstruction DropInBackYard = new DeliveryInstruction(9, "Drop in Back Yard", "BACK");
        public static readonly DeliveryInstruction DropInFrontYard = new DeliveryInstruction(10, "Drop in Front Yard", "FRONT");
        public static readonly DeliveryInstruction DropInGarage = new DeliveryInstruction(11, "Drop in Garage", "GARAGE");
        public static readonly DeliveryInstruction DropInHouse = new DeliveryInstruction(12, "Drop in House", "INHOUSE");
        public static readonly DeliveryInstruction DropOnDriveway = new DeliveryInstruction(13, "Drop on Driveway", "DRIVEWAY");
        public static readonly DeliveryInstruction DropOnSidewalk = new DeliveryInstruction(14, "Drop on Sidewalk", "SIDEWALK");
        public static readonly DeliveryInstruction InstallMaterial = new DeliveryInstruction(15, "Install Material", "INSTALL");
        public static readonly DeliveryInstruction MeasureThisDay = new DeliveryInstruction(16, "Measure This Day", "MEASURE");
        public static readonly DeliveryInstruction PMDelivery = new DeliveryInstruction(17, "PM Delivery", "PMDELIV");
        public static readonly DeliveryInstruction SameDayDelivery = new DeliveryInstruction(18, "Same Day Delivery", "SAMEDAY");
        public static readonly DeliveryInstruction ShipWithOriginalOrder = new DeliveryInstruction(19, "Ship with Original Order", "WITHOS");
    }
}