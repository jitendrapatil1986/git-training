using System;

namespace Warranty.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ToLastDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 23, 59, 59);
        }

        public static DateTime ToFirstDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

    }
}
