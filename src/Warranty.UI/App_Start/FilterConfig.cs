using Warranty.Core.Enumerations;
using Warranty.UI.Core.Initialization;

namespace Warranty.UI
{
    using System.Web.Mvc;

    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new RoleAuthorizeAttribute(UserRoles.Access));
            filters.Add(new HandleErrorAttribute());
        }
    }
}