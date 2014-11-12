namespace Warranty.Core.Enumerations
{
    public class ToDoPriority : Enumeration<ToDoPriority>
    {
        private ToDoPriority(int value, string displayName)
            : base(value, displayName)
        {
        }

        public static readonly ToDoPriority High = new ToDoPriority(1, "High");
        public static readonly ToDoPriority Medium = new ToDoPriority(2, "Medium");
        public static readonly ToDoPriority Low = new ToDoPriority(3, "Low");
    }
}
