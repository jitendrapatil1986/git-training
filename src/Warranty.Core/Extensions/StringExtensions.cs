namespace Warranty.Core.Extensions
{
    public static class StringExtensions
    {
        public static string Truncate(this string stringToTruncate, int maxLenght)
        {
            return stringToTruncate.Length > maxLenght ? stringToTruncate.Substring(0, maxLenght) : stringToTruncate;
        }

        public static bool IsJDEWildCard(this string stringToCheck)
        {
            return stringToCheck == null || (string.IsNullOrWhiteSpace(stringToCheck) || stringToCheck.Contains("*"));
        }

        public static string SafeTrim(this string stringToTrim)
        {
            return stringToTrim == null ? null : stringToTrim.Trim();
        }

        public static string ToHomeownerNumber(this string value)
        {
            // Homeowner Numbers stored in Purchasing do not have leading zeros
            return value.WithoutLeadingZeros();
        }

        public static string ToVendorNumber(this string value)
        {
            // Vendor Numbers stored in Purchasing do not have leading zeros
            return value.WithoutLeadingZeros();
        }

        private static string WithoutLeadingZeros(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? value : value.TrimStart('0');
        }

        public static byte[] ToByteArray(this string value)
        {
            var encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(value);
        }
    }
}
