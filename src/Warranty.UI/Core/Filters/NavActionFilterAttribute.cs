namespace Warranty.UI.Core.Filters
{
    using System.Web.Mvc;

    public class CurrentNavSectionAttribute : ActionFilterAttribute
    {
        public const string ViewDataKey = "CurrentNavSectionAttributeData";

        public string NavSectionName { get; private set; }

        public CurrentNavSectionAttribute(string navSectionName)
        {
            NavSectionName = navSectionName;
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var viewResultBase = filterContext.Result as ViewResultBase;

            if (viewResultBase != null)
                viewResultBase.ViewData[ViewDataKey] = NavSectionName;

            base.OnActionExecuted(filterContext);
        }
    }

    public class CurrentSubNavSectionAttribute : ActionFilterAttribute
    {
        public const string ViewDataKey = "CurrentSubNavSectionAttributeData";

        private readonly string _subNavSectionName;

        public CurrentSubNavSectionAttribute(string subNavSectionName)
        {
            _subNavSectionName = subNavSectionName;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var viewResultBase = filterContext.Result as ViewResultBase;

            if (viewResultBase != null)
                viewResultBase.ViewData[ViewDataKey] = _subNavSectionName;

            base.OnActionExecuted(filterContext);
        }
    }
}