namespace Warranty.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableExtensions
    {
        public static string CommaSeparate<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable != null)
                return string.Join(",", enumerable);

            return null;
        }

        public static string CommaSeparateWrapWithSingleQuote<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Select(x => "'" + x + "'").CommaSeparate();
        }

        public static string Truncate(this string stringToTruncate, int maxLength)
        {
            return stringToTruncate.Length > maxLength ? stringToTruncate.Substring(0, maxLength) : stringToTruncate;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }

        public static IEnumerable<T> Concat<T>(this T first, IEnumerable<T> enumerable)
        {
            yield return first;

            foreach (var obj in enumerable)
                yield return obj;
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> enumerable, T last)
        {
            foreach (var obj in enumerable)
                yield return obj;

            yield return last;
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> enumerable, params T[] list)
        {
            return Enumerable.Concat(enumerable, list);
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var obj in enumerable)
                action(obj);
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> enumerable, T itemToIgnore)
        {
            return enumerable.Except(new[] { itemToIgnore });
        }

        public static IEnumerable<T> AsEnumerable<T>(this T item)
        {
            return new[] { item };
        }

        public static int IndexOf<T>(this IEnumerable<T> enumerable, T value) where T : IComparable
        {
            var i = 0;

            foreach (var item in enumerable)
            {
                if (item.CompareTo(value) == 0)
                    return i;

                ++i;
            }

            return -1;
        }
    }
}