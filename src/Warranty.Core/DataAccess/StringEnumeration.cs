using System;
using System.Diagnostics;
using Headspring;
using Warranty.Core.Enumerations;

namespace Warranty.Core.DataAccess
{
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