namespace Warranty.Core.Enumerations
{
    using System;
    using System.Diagnostics;

    [Serializable]
    [DebuggerDisplay("{DisplayName} - {Value}")]
    public abstract class Int32Enumeration<TEnumeration> : Enumeration<TEnumeration, int>, IEnumeration<TEnumeration, int>
        where TEnumeration : Int32Enumeration<TEnumeration>
    {
        protected Int32Enumeration(int value, string displayName)
            : base(value, displayName)
        {
        }

        public static TEnumeration FromInt32(int value)
        {
            return FromValue(value);
        }

        public static bool TryFromInt32(int listItemValue, out TEnumeration result)
        {
            return TryFromValue(listItemValue, out result);
        }
    }
}