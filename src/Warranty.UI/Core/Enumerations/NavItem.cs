using Headspring;

namespace Warranty.UI.Core.Enumerations
{
    public class NavItem : Enumeration<NavItem>
    {
        public static readonly NavItem Home = new NavItem(4, Display.HomeName, Display.HomeName);

        public NavItem(int value, string displayName, string title)
            : base(value, displayName)
        {
            Title = title;
        }

        public string Title { get; private set; }

        public static class Display
        {
            public const string HomeName = "Home";
        }
    }
}