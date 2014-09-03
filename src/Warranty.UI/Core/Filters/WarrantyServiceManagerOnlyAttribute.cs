namespace Warranty.UI.Core.Filters
{
    using System.Web;
    using System.Web.Mvc;
    using Warranty.Core.Enumerations;

    public class WarrantyServiceManagerOnlyAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return httpContext.User.IsInRole(UserRoles.WarrantyServiceManager);
        }
    }
}