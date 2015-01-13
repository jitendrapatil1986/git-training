using System.Web;
using System.Web.Mvc;
using Warranty.Core.Enumerations;

namespace Warranty.UI.Core.Filters
{
    public class WarrantyServiceCoordinatorOnlyAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return httpContext.User.IsInRole(UserRoles.WarrantyServiceCoordinator) || httpContext.User.IsInRole(UserRoles.CustomerCareManager);
        }
    }
}