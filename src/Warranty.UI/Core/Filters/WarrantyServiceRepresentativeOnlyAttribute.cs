namespace Warranty.UI.Core.Filters
{
    using System.Web;
    using System.Web.Mvc;
    using Warranty.Core.Enumerations;

    public class WarrantyServiceRepresentativeOnlyAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return httpContext.User.IsInRole(UserRoles.WarrantyServiceRepresentative) && (!httpContext.User.IsInRole(UserRoles.WarrantyServiceCoordinator) && !httpContext.User.IsInRole(UserRoles.WarrantyServiceManager));
        }
    }
}