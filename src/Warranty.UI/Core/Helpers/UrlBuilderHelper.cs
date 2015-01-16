namespace Warranty.UI.Core.Helpers
{
    using System.Configuration;
    using System.Web;
    using System.Web.Mvc;

    public static class UrlBuilderHelper
    {
        public static string GetUrl(string controller, string action, object routingParams)
        {
            var url = ConfigurationManager.AppSettings["Warranty.BaseUri"];
            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            url += urlHelper.Action(action, controller, routingParams);
            return url;
        }
    }
}