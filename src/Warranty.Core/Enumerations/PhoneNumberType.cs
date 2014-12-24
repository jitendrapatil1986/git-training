namespace Warranty.Core.Enumerations
{
    public class PhoneNumberType : Enumeration<PhoneNumberType>
    {
        private PhoneNumberType(int value, string displayName, string icon)
            : base(value, displayName)
        {
            Icon = icon;
        }

        public static readonly PhoneNumberType Home = new PhoneNumberType(1, "Home", "earphone");
        public static readonly PhoneNumberType Mobile = new PhoneNumberType(2, "Mobile", "phone");

        public string Icon { get; set; }
    }
}