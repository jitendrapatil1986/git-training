namespace Warranty.Core.Enumerations
{
    public class ItemClassification : Enumeration<ItemClassification>
    {
        private ItemClassification(int value, string displayName)
            :base(value, displayName)
        {
        }
         
        public static readonly ItemClassification BuilderIncomplete = new ItemClassification(1, "Builder Incomplete");
        public static readonly ItemClassification CustomerGoodwill = new ItemClassification(2, "Customer Goodwill");
        public static readonly ItemClassification Warrantable = new ItemClassification(3, "Warrantable");
        public static readonly ItemClassification NoAction = new ItemClassification(4, "No Action");
    }
}