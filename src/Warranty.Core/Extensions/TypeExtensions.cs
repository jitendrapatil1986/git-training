using System;
using Warranty.Core.Enumerations;

namespace Warranty.Core.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsEnumeration(this Type memberInfo)
        {
            return memberInfo.BaseType != null
            && memberInfo.BaseType.IsGenericType
            && memberInfo.BaseType.GetGenericTypeDefinition() == typeof(Enumeration<>);
        }
    }
}
