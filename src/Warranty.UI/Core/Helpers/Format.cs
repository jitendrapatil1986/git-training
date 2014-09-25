using System.Globalization;
using Warranty.Core.Enumerations;

namespace Warranty.UI.Core.Helpers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Warranty.Core.Extensions;

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
            return String.Format("{0:MM/dd}", date);
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

        public static string DateForServiceCallWiget(DateTime? date)
        {
            return String.Format("{0:MMM dd yyyy}", date);
        }

        public static MvcHtmlString DaysOpenedFor(int days, DateTime openedDate, DateTime? closedDate)
        {
            string cssClass;
            if (days == 7)
                cssClass = "seven-day";
            else if (days < 7)
                cssClass = "less-seven-day";
            else
                cssClass = "eight-plus-day";

            var stringDays = days == 1 ? "Day" : "Days";

            var htmlString =
                string.Format(
                    @"<div class='opened-for opened-for-{0} has-bottom-tooltip' data-original-title='From {1} to {2}'>{3}<p>{4}</p></div>",
                    cssClass,
                    String.Format("{0:MMM dd yyyy}", openedDate),
                    String.Format("{0:MMM dd yyyy}", closedDate),
                    days,
                    stringDays);

            return MvcHtmlString.Create(htmlString);
        }

        public static MvcHtmlString Escalated(bool isEscalated, string reason, DateTime? escalatedDate)
        {
            var htmlString = String.Empty;


            if (isEscalated)
                htmlString =
                    string.Format(
                        @"<div class='has-bottom-tooltip text-center' data-original-title='{0}'><span class='glyphicon glyphicon-fire'></span><br/>{1}</div>",
                        reason, DateMonthDayYear(escalatedDate));

            return MvcHtmlString.Create(htmlString);
        }

        public static MvcHtmlString SpecialProject(bool isSpecialProject)
        {
            var htmlString = String.Empty;
            if (isSpecialProject)
                htmlString = @"<div class='special-project has-bottom-tooltip' data-original-title='Special Project'>S</div>";

            return MvcHtmlString.Create(htmlString);
        }

        public static MvcHtmlString YearsWithinWarranty(int years, DateTime warrantyStartDate)
        {
            string cssClass;
            if (years <= 1)
                cssClass = "warranty-one-year";
            else if (years == 2)
                cssClass = "warranty-two-year";
            else if(years < 10)
                cssClass = "warranty-three-nine-year";
            else
                cssClass = "warranty-ten-plus-year";

            var stringYears = years <= 1 ? "Year" : "Years";

            var displayedYears = years == 0 ? "1" : years.ToString(CultureInfo.InvariantCulture);

            var toolTip = string.Format("Warranty Start Date: {0}", String.Format("{0:MMM dd yyyy}", warrantyStartDate));

            var htmlString = string.Format(@"<span class='label label-{0} has-bottom-tooltip' title='{1}'>{2} {3}</span>",
                                            cssClass, 
                                            toolTip, 
                                            displayedYears, 
                                            stringYears);

            return MvcHtmlString.Create(htmlString);
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

        public static MvcHtmlString NumberOfLineItems(int numberOfLineItems)
        {
            if (numberOfLineItems <= 1)
                return MvcHtmlString.Empty;

            return MvcHtmlString.Create(string.Format("<span class=\"label label-info has-bottom-tooltip\" title=\"Number of line items\">{0}</span>", numberOfLineItems));
        }

        public static MvcHtmlString ShowRedLabelIfCountGreaterThanZero(int numberOfLines)
        {
            if (numberOfLines <= 0)
                return MvcHtmlString.Create(string.Format("<span class=\"label label-default\">{0}</span>", numberOfLines));

            return MvcHtmlString.Create(string.Format("<span class=\"label label-danger\">{0}</span>", numberOfLines));
        }

        public static MvcHtmlString ServiceCallDaysLeft(int numberOfDaysRemaining)
        {
            var plural = "s";
            var css = "text-muted";
            if (numberOfDaysRemaining == 1)
                plural = "";
            if (numberOfDaysRemaining <= 2)
            {
                css = "text-danger";
            }

            const string daysLeft = "<strong class=\"{0}\">{1}</strong> Day{2} Left";
            return MvcHtmlString.Create(string.Format(daysLeft, css, numberOfDaysRemaining, plural));
        }

        public static MvcHtmlString PhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return MvcHtmlString.Empty;
            }

            return MvcHtmlString.Create("<span class=\"glyphicon glyphicon-earphone text-muted\"></span> " + phoneNumber);
        }

        public static MvcHtmlString EditablePhoneNumber(PhoneNumberType phoneNumberType, string phoneNumber, string postUrl, Guid serviceCallId)
        {
            var htmlString =
                string.Format("<div class='inline-reassign-text'><a class='glyphicon glyphicon-{0} text-muted visible-xs' href='tel:{4}'></a><span class='glyphicon glyphicon-{0} text-muted hidden-xs'></span> <a href='#' class='hidden-xs phone-number-with-extension' data-type='text' data-emptytext='Add' id='{1}_Phone' data-url='{2}' data-pk='{3}' data-clear='false' data-value='{4}'>{4}</a></div>", phoneNumberType.Icon, phoneNumberType.DisplayName, postUrl, serviceCallId, phoneNumber);

            return MvcHtmlString.Create(htmlString);
        }

        public static MvcHtmlString EditableEmail(string email, string postUrl, Guid serviceCallId, string callNumber)
        {
            return
                MvcHtmlString.Create(
                    string.Format(@"<div class='inline-reassign-text'><a class='glyphicon glyphicon-envelope text-muted' href='mailto:{2}?subject=David Weekley Homes Warranty Service Call %23 {3}'></a> 
                                    <a href='mailto:{2}?subject=David Weekley Homes Warranty Service Call %23 {3}' class='hidden-xs' data-type='text' data-emptytext='Add' id='Email' data-url='{0}' data-pk='{1}' data-clear='false' data-value='{2}'>{2}</a></div>", postUrl, serviceCallId, email, callNumber));
        }

        public static MvcHtmlString CellNumber(string cellNumber)
        {
            if (string.IsNullOrWhiteSpace(cellNumber))
            {
                return MvcHtmlString.Empty;
            }

            return MvcHtmlString.Create("<span class=\"glyphicon glyphicon-phone text-muted\"></span> " + cellNumber);
        }

        public static MvcHtmlString HomeNumber(string homeNumber)
        {
            if (string.IsNullOrWhiteSpace(homeNumber))
            {
                return MvcHtmlString.Empty;
            }

            return MvcHtmlString.Create("<span class=\"glyphicon glyphicon-home text-muted\"></span> " + homeNumber);
        }

        public static MvcHtmlString Email(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return MvcHtmlString.Empty;
            }

            return MvcHtmlString.Create("<span class=\"glyphicon glyphicon-envelope text-muted\"></span> " + email);
        }

        public static string CityStateZip(string city, string state, string zip)
        {
            return city + ", " + state + " " + zip;
        }

        public static MvcHtmlString ServiceCallStatus(ServiceCallStatus serviceCallStatus)
        {
            var status = serviceCallStatus.DisplayName;
            var htmlString = string.Format("<span class='label label-{0}-service-call'>{1}</span>", status.ToLower(),
                                           status);
            return MvcHtmlString.Create(htmlString);
        }

        public static MvcHtmlString ActionwithPopup(string id, string url, Guid serviceCallId)
        {
            var html = string.Format(@"<div id='{0}' class='popup-action-with-message'>
                            <p><small>Reason:</small></p>
                            <textarea rows='5'></textarea>
                            <a href='#' class='btn btn-primary btn-sm pull-left btn-execute-action' data-action-url='{1}' data-service-call-id='{2}'>Submit</a>
                            <a href='#' class='btn-link btn-sm pull-right btn-cancel-popup'>Cancel</a>
                        </div>", id, url, serviceCallId);
            return MvcHtmlString.Create(html);
        }

        public static MvcHtmlString ServiceCallToggleButton(string id, bool shouldToggle, string inactiveText, string activeText)
        {
            var currentText = !shouldToggle ? inactiveText : activeText;
            var nextText = shouldToggle ? inactiveText : activeText;

            return
                MvcHtmlString.Create(
                    string.Format(
                        "<a href='#' class='btn btn-default pull-right btn-action-with-popup' data-action-with-popup='{0}' id='btn_{0}' data-next-text='{1}'>{2}</a>",
                        id, nextText, currentText));
        }

        public static MvcHtmlString HomeOwner(string homeownerName, int homeownerNumber)
        {
            if (homeownerNumber == 1)
                return MvcHtmlString.Create(homeownerName);

            var html = string.Format("{0} <span class='label label-info muted'>{1}</span>", homeownerName, homeownerNumber.ToOrdinalSuffixed());
            return MvcHtmlString.Create(html);
        }
    }
}