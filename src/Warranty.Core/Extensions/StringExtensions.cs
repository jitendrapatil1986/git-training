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

        public static string ToPhoneNumberWithExtension(this string phoneNumber)
        {
            phoneNumber = CleanPhoneNumber(phoneNumber);

            if (phoneNumber == null || phoneNumber.Length < 10)
                return string.Empty;

            var area = phoneNumber.Substring(0, 3);
            var major = phoneNumber.Substring(3, 3);
            var minor = phoneNumber.Substring(6, 4);
            var extension = " x" + phoneNumber.Substring(10, phoneNumber.Length - 10);

            var format = "({0}) {1}-{2}" + (extension.Length > 2 ? extension : string.Empty);

            return string.Format(format, area, major, minor);
        }
    }
}
