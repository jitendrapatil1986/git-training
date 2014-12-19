namespace Warranty.Core.Enumerations
{
    public class HomeownerVerificationType: Enumeration<HomeownerVerificationType>
    {
        private HomeownerVerificationType(int value, string displayName)
            : base(value, displayName)
        {
        }

        public static readonly HomeownerVerificationType Signature = new HomeownerVerificationType(1, "Signature");
        public static readonly HomeownerVerificationType Email = new HomeownerVerificationType(2, "Email");
        public static readonly HomeownerVerificationType PhoneCall = new HomeownerVerificationType(3, "Phone Call");
        public static readonly HomeownerVerificationType NoResponse = new HomeownerVerificationType(4, "No Response");
        public static readonly HomeownerVerificationType NotVerified = new HomeownerVerificationType(5, "Not Verified");
        public static readonly HomeownerVerificationType Imported = new HomeownerVerificationType(6, "Imported");
    }
}