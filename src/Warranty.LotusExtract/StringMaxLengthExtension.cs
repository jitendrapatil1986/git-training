namespace Warranty.LotusExtract
{
    public static class StringMaxLengthExtension
    {
        public static string MaxLength(this string input, int maxLength)
        {
            return input.Substring(0, (input.Length < maxLength) ? input.Length : maxLength);
        }
    }
}
