using System;
using System.Linq;

namespace Warranty.Core.Enumerations
{
    public abstract class JdeEnumeration<TEnum> : Enumeration<TEnum> where TEnum : JdeEnumeration<TEnum>
    {
        protected JdeEnumeration(int value, string displayName, string jdeCode)
            : base(value, displayName)
        {
            if (jdeCode == null)
                throw new ArgumentNullException("jdeCode");

            JdeCode = jdeCode;
        }

        public string JdeCode { get; set; }

        public static TEnum FromJdeCode(string code)
        {
            return GetAll().SingleOrDefault(x => x.JdeCode.Equals(code));
        }
    }
}