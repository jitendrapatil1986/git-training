namespace Warranty.Core.Enumerations
{
    public class PaymentTypes : Enumeration<PaymentTypes>
    {
        public PaymentTypes(int value, string displayName)
            :base(value, displayName)
        {
        }

        public static readonly PaymentTypes Payment = new PaymentTypes(0, "Payment");
        public static readonly PaymentTypes StandAloneBackcharge = new PaymentTypes(1, "Stand Alone Backcharge");
    }
}