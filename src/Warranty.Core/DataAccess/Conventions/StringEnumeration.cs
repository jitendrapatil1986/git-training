namespace Warranty.Core.DataAccess.Conventions
{
    using System;
    using System.Diagnostics;
    using Enumerations;

    [Serializable]
    [DebuggerDisplay("{DisplayName} - {Value}")]
    public abstract class StringEnumeration<TEnumeration> : Enumeration<TEnumeration, string>, IEnumeration<TEnumeration, string>, IStringEnumeration
        where TEnumeration : StringEnumeration<TEnumeration>
    {
        protected StringEnumeration(string value, string displayName)
            : base(value, displayName)
        {
        }
    }
}