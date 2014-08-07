namespace Warranty.UI.Core.Helpers
{
    using System;
    using System.Web.Mvc;

    public static class Format
    {
        public static string Money(decimal amount)
        {
            return String.Format("{0:C2}", amount);
        }

        public static string Money(decimal? amount)
        {
            return String.Format("{0:C2}", amount);
        }

        public static string MoneyWithoutZero(decimal amount)
        {
            if (amount == 0)
                return "";

            return String.Format("{0:C2}", amount);
        }

        public static string MoneyWithoutZero(decimal? amount)
        {
            return amount.HasValue ? MoneyWithoutZero(amount.Value) : "";
        }

        public static string MoneyWithoutDecimal(decimal amount)
        {
            return String.Format("{0:C0}", amount);
        }

        public static string MoneyWithoutDecimal(decimal? amount)
        {
            return String.Format("{0:C0}", amount);
        }

        public static string Dollars(decimal amount)
        {
            if (amount.ToString("$###,###,###,###,##0.##").Contains("."))
                return amount.ToString("$###,###,###,###,##0.00");
            return amount.ToString("$###,###,###,###,##0.##");
        }

        public static string Dollars(decimal? amount)
        {
            return !amount.HasValue ? "" : Dollars(amount.Value);
        }

        public static string DateWeekDayAndMonthDay(DateTime date)
        {
            return string.Format("{0:ddd MM/dd}", date);
        }

        public static string DateMonthDay(DateTime date)
        {
            return String.Format("{0:M/d}", date);
        }

        public static string DateMonthDay(DateTime? date)
        {
            if (date == null)
                return "";
            return String.Format("{0:M/d}", date);
        }

        public static string Date(DateTime date)
        {
            return date == System.DateTime.MinValue ? "" : String.Format("{0:d}", date);
        }

        public static string Date(DateTime? date)
        {
            return !date.HasValue ? "" : Date(date.Value);
        }

        public static string DateMonthDayYear(DateTime? date)
        {
            return date == null ? "" : date.Value.ToString("MM/dd/yyyy");
        }

        public static string DateFullMonthDayYear(DateTime? date)
        {
            if (date == null)
                return "";

            return date.Value.ToString("MMMM dd, yyyy");
        }

        public static string DateForTaskWiget(DateTime date)
        {
            return String.Format("{0:MMM dd yyyy}", date);
        }

        public static string DateAsMonthDayOnly(DateTime date)
        {
            return String.Format("{0:MMM d}", date);
        }

        public static string DateAsNumericalMonthDayOnly(DateTime date)
        {
            return String.Format("{0:M/d}", date);
        }

        public static string DateTime(DateTime dateTime)
        {
            return dateTime == System.DateTime.MinValue ? "" : String.Format("{0:M/d/yy h:mm tt}", dateTime);
        }

        public static string DateTime(DateTime? dateTime)
        {
            return !dateTime.HasValue ? "" : DateTime(dateTime.Value);
        }

        public static string YesNo(bool value)
        {
            return value ? "Yes" : "No";
        }

        public static string YesNo(bool? value)
        {
            return !value.HasValue ? "" : YesNo(value.Value);
        }

        public static string YesNo(int value)
        {
            return value == 1 ? "Yes" : "No";
        }

        public static string YesNo(int? value)
        {
            return !value.HasValue ? "" : YesNo(value.Value);
        }

        public static string FileSize(long bytes, int decimals = 1)
        {
            if (bytes == 0)
            {
                return "0 B";
            }

            var sizes = new[] { "B", "KB", "MB", "GB", "TB", "PB" };

            var order = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));

            var num = Math.Round(bytes / Math.Pow(1024, order), decimals);

            return String.Format("{0} {1}", num, sizes[order]);
        }

        public static string Percentage(decimal? value)
        {
            return value.HasValue ? value.Value.ToString("##0.00") + "%" : String.Empty;
        }

        public static string WrapEmailAddress(string email)
        {
            var result = email;

            if (email.Length > 25)
            {
                var tempEmail = email;
                var idxAt = tempEmail.IndexOf("@") + 1;
                result = (tempEmail.Substring(0, idxAt) + "<br/>" + tempEmail.Substring(idxAt, (tempEmail.Length - idxAt)));
            }

            return result;
        }

        public static string TruncateWithElipse(string text, int maxLength)
        {
            if (text.Length < maxLength)
                return text;

            return text.Substring(0, maxLength - 3).TrimEnd() + "...";
        }

        public static string WithParentheses(string valueToWrap)
        {
            if (string.IsNullOrWhiteSpace(valueToWrap))
                return "";

            return "(" + valueToWrap + ")";
        }

        public static string NotSpecifiedIfNullOrEmpty(string textToCheck)
        {
            return string.IsNullOrEmpty(textToCheck) ? "Not Specified" : textToCheck.Trim();
        }
        
        public static string InvoiceNumber(string invoiceNumber)
        {
            return string.IsNullOrEmpty(invoiceNumber) ? "" : invoiceNumber.ToUpper();
        }

        public static MvcHtmlString DaysAheadBehind(int days)
        {
            string cssClass;
            if (days == 0)
                cssClass = "";
            else if (days > 0)
                cssClass = "success";
            else
                cssClass = "important";

            return new MvcHtmlString(string.Format(@"<span class=""badge badge-{0}"">{1}</span>", cssClass, days));
        }

        public static MvcHtmlString TextWithNewLine(string text)
        {
            return MvcHtmlString.Create(text.Replace(Environment.NewLine, "<br/>"));
        }

        public static MvcHtmlString TrimWithTooltip(string text, int length, bool includeZoomIcon = true)
        {
            if (string.IsNullOrWhiteSpace(text))
                return MvcHtmlString.Empty;

            var trimmedText = "";
            if (length > 0)
            {
                if (text.Length <= length)
                    return new MvcHtmlString(text);

                trimmedText = text.Substring(0, length) + "&hellip;";
            }

            var icon = (includeZoomIcon && trimmedText != text) ? @"<i class=""icon icon-small icon-zoom-in muted""></i>" : "";

            return new MvcHtmlString(string.Format(@"<span class=""has-bottom-tooltip"" title=""{1}"">{0}{2}</span>", trimmedText, text, icon));
        }

        public static MvcHtmlString WithTooltip(string text, string tooltip, bool includeZoomIcon = true)
        {
            if (string.IsNullOrWhiteSpace(text))
                return MvcHtmlString.Empty;

            var icon = includeZoomIcon ? @"<i class=""icon icon-small icon-zoom-in muted""></i>" : "";

            return new MvcHtmlString(string.Format(@"<span class=""has-bottom-tooltip"" title=""{0}"">{1} {2}</span>", tooltip, text, icon));
        }
    }
}