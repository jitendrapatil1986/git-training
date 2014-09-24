using System.Linq;

namespace Warranty.Core.Extensions
{
    public static class IntExtensions
    {
        public static string ToOrdinalSuffixed(this int i)
        {
            var suffix = "th";
            var number = i.ToString().ToCharArray().Reverse().ToArray();

            if (number.Length == 1 || number[1] != '1')
            {
                switch (number[0])
                {
                    case '1':
                        suffix = "st";
                        break;
                    case '2':
                        suffix = "nd";
                        break;
                    case '3':
                        suffix = "rd";
                        break;
                }
            }

            return i + suffix;
        }
    }
}
