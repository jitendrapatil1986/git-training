using System.Web.Mvc;
using Warranty.Core.Enumerations;

namespace Warranty.UI.Core.Filters
{
    public class WarrantyAuthorize : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.IsInRole(UserRoles.WarrantyServiceCoordinator) || AuthorizeCore(filterContext.HttpContext)) return;

            filterContext.Result = new ViewResult { ViewName = "AuthorizationError" };
        }
    }
}