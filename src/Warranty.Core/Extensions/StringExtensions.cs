namespace Warranty.Core.Extensions
{
    using System;
    using System.Text.RegularExpressions;

    public static class StringExtensions
    {
        private static readonly Regex PascalCaseToSpacesRegex = new Regex(@"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", RegexOptions.Compiled);

        public static string SplitTitleCase(this string val)
        {
            return PascalCaseToSpacesRegex.Replace(val, " $1");
        }

        public static string CleanPhoneNumber(this string val)
        {
            return val == null ? string.Empty : val.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "").Replace("x", "").Trim();
        }
    }
}
