namespace Warranty.UI.Core.Helpers
{
    using System;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using Enumerations;

    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString RenderToastMessage(this HtmlHelper htmlHelper)
        {
            var toastMessage = MvcHtmlString.Empty;

            if (!htmlHelper.ViewContext.ViewData.ModelState.IsValid)
            {
                toastMessage = BuildToastMessage(ToastType.Error.Type, htmlHelper.BuildGeneralValidationSummary());
            }

            if (htmlHelper.ViewContext.TempData.ContainsKey(SystemConstants.ToastIdentifier))
            {
                var toastData = ToastType.SplitTempDataValue(htmlHelper.ViewContext.TempData[SystemConstants.ToastIdentifier].ToString());
                toastMessage = BuildToastMessage(toastData.Key, toastData.Value);
            }

            return new MvcHtmlString(toastMessage.ToHtmlString());
        }

        private static string BuildGeneralValidationSummary(this HtmlHelper htmlHelper)
        {
            var validationSummary = htmlHelper.ValidationSummary(true, "Sorry, there was a problem submitting the form.").ToHtmlString();

            var tagline = new TagBuilder("span");
            tagline.AddCssClass("validation-summary-error-tagline");
            tagline.SetInnerText("You'll find more details highlighted below.");

            return validationSummary + tagline.ToString(TagRenderMode.Normal);
        }
        
        private static MvcHtmlString BuildToastMessage(string toastType, string toastMessage)
        {
            if (toastMessage != null)
            {
                var hidden = new TagBuilder("input");
                hidden.GenerateId("toast-" + Guid.NewGuid());
                hidden.AddCssClass(SystemConstants.ToastIdentifier);

                hidden.Attributes.Add("type", "hidden");
                hidden.Attributes.Add("data-toast-type", toastType);
                hidden.Attributes.Add("value", toastMessage);

                return MvcHtmlString.Create(hidden.ToString(TagRenderMode.SelfClosing));
            }

            return MvcHtmlString.Empty;
        }
         
    }
}