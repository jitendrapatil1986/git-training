using System;
using System.Linq.Expressions;
using Yay.Enumerations;

namespace Warranty.Core.Services
{
    public static class ReflectionService
    {
        public static string GetPropertyName<T>(Expression<Func<T, object>> expression)
        {
            return expression.GetPropertyString();
        }

        public static string GetPropertyId<T>(Expression<Func<T, object>> expression)
        {
            return expression.GetPropertyString().Replace(".", "_");
        }
    }
}