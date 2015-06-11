using Common.Security.Queries;
using Common.UI.Security.Session;

namespace Warranty.UI.Controllers
{
    public class SecurityController : Common.UI.Security.Controllers.SecurityController
    {
        public SecurityController(IWebUserSession userSession, UserSearchQuery searchQuery): base(userSession, searchQuery)
        {
            DefaultRedirectActionName = "Index";
            DefaultRedirectControllerName = "Home";
        }

    }
}
