namespace Warranty.Core.Enumerations
{
    public class HomeownerContactType : Enumeration<HomeownerContactType>
    {
        private HomeownerContactType(int value, string displayName)
            :base(value, displayName)
        {
        }

        public static HomeownerContactType Phone = new HomeownerContactType(1, "Phone");
        public static HomeownerContactType Email = new HomeownerContactType(2, "Email");
    }
}