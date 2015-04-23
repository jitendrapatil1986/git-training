using System.Web;
using System.Web.WebPages;

namespace Warranty.UI.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using Enumerations;
    using HtmlTags;
    using Initialization;
    using Microsoft.Practices.ServiceLocation;
    using Warranty.Core;
    using Warranty.Core.Entities;
    using Warranty.Core.Features.ProblemCodes;
    using Warranty.Core.Services;

    public static class HtmlHelperExtensions
    {
        public static IEnumerable<string> GetSystemMessages(this HtmlHelper htmlHelper)
        {
            ISystemMonitor surveymonitor = IoC.Container.GetInstance<SurveyClientSystemMonitor>();
            ISystemMonitor accountingmonitor = IoC.Container.GetInstance<AccountingClientSystemMonitor>();
            return surveymonitor.Messages.Concat(accountingmonitor.Messages);
        }

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

            var systemMessages = htmlHelper.GetSystemMessages().Aggregate("", (current, message) => current + BuildToastMessage(ToastType.Warning.Type, message));

            return new MvcHtmlString(toastMessage + systemMessages);
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

        public static HtmlTag ProblemCodeDropdown<T>(this HtmlHelper<T> html, ProblemCode currentProblemCode, string valueObservableName) where T : class
        {
            SelectTag select;
            var mediator = ServiceLocator.Current.GetInstance<IMediator>();

                var problemCodes = mediator.Request(new ProblemCodesQuery());

                select = new SelectTag(t =>
                {
                    t.Option("Select Problem Code", string.Empty);
                    foreach (var problemCode in problemCodes)
                    {
                        var htmlTag = t.Option(problemCode.Text, problemCode.Value);
                        if (currentProblemCode != null && currentProblemCode.JdeCode == problemCode.Value)
                            htmlTag.Attr("selected");
                    }
                });

            select.Id("problemCode");
            select.Attr("name", "SelectProblemCode");
            select.AddClass("form-control");
            select.Attr("data-bind", string.Format("value: {0}",valueObservableName));
            return select;
        }

         public static MvcHtmlString ServiceCallActions(this HtmlHelper htmlHelper, Guid serviceCallId)
         {
             return MvcHtmlString.Empty;

             var buttonSpan = new TagBuilder("span");
             buttonSpan.AddCssClass("glyphicon");
             buttonSpan.AddCssClass("glyphicon-cog");

             var button = new TagBuilder("button");
             button.Attributes.Add("type", "button");
             button.Attributes.Add("data-toggle", "dropdown");
             button.AddCssClass("btn");
             button.AddCssClass("btn-default");
             button.AddCssClass("dropdown-toggle");
             button.AddCssClass("pull-right");
             button.InnerHtml = buttonSpan.ToString(TagRenderMode.Normal);

             var menu = new TagBuilder("ul");
             menu.AddCssClass("dropdown-menu");
             menu.Attributes.Add("role", "menu");

             var reassignLink = new TagBuilder("li")
             {
                 InnerHtml = htmlHelper.ActionLink("Reassign", "Reassign", "ServiceCall", new {id = serviceCallId}, null).ToHtmlString()
             };
             var requestPaymentLink = new TagBuilder("li")
             {
                 InnerHtml = htmlHelper.ActionLink("Request Payment", "RequestPayment", "ServiceCall", new {id = serviceCallId}, null).ToHtmlString()
             };
             var addNote = new TagBuilder("li")
             {
                 InnerHtml = htmlHelper.ActionLink("Add Note", "AddNote", "ServiceCall", new {id = serviceCallId}, null).ToHtmlString()
             };
             var closeCall = new TagBuilder("li")
             {
                 InnerHtml = htmlHelper.ActionLink("Close", "Close", "ServiceCall", new {id = serviceCallId}, null).ToHtmlString()
             };

             menu.InnerHtml = reassignLink.ToString(TagRenderMode.Normal) + requestPaymentLink.ToString(TagRenderMode.Normal) + addNote.ToString(TagRenderMode.Normal) + closeCall.ToString(TagRenderMode.Normal);

             return MvcHtmlString.Create(button.ToString(TagRenderMode.Normal) + menu.ToString(TagRenderMode.Normal));
         }

         public static MvcHtmlString Script(this HtmlHelper htmlHelper, Func<object, HelperResult> template)
         {
             htmlHelper.ViewContext.HttpContext.Items["_script_" + Guid.NewGuid()] = template;
             return MvcHtmlString.Empty;
         }

         public static IHtmlString RenderScripts(this HtmlHelper htmlHelper)
         {
             foreach (object key in htmlHelper.ViewContext.HttpContext.Items.Keys)
             {
                 if (key.ToString().StartsWith("_script_"))
                 {
                     var template = htmlHelper.ViewContext.HttpContext.Items[key] as Func<object, HelperResult>;
                     if (template != null)
                     {
                         htmlHelper.ViewContext.Writer.Write(template(null));
                     }
                 }
             }
             return MvcHtmlString.Empty;
         }
    }
}