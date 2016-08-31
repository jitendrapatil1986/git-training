using System;
using System.Collections.Generic;
using System.Linq;
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

        public static bool IsA(this Type type, Type typeToBe)
        {
            if (!typeToBe.IsGenericTypeDefinition)
                return typeToBe.IsAssignableFrom(type);

            var toCheckTypes = new List<Type> { type };
            if (typeToBe.IsInterface)
                toCheckTypes.AddRange(type.GetInterfaces());

            var basedOn = type;
            while (basedOn.BaseType != null)
            {
                toCheckTypes.Add(basedOn.BaseType);
                basedOn = basedOn.BaseType;
            }

            return toCheckTypes.Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeToBe);
        }
    }
}
