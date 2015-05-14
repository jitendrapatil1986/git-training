using Common.Security.Queries;
using Common.Security.User.Session;

namespace Warranty.UI.Controllers
{
    public class SecurityController : Common.Security.Controllers.SecurityController
    {
        public SecurityController(IWebUserSession userSession, UserSearchQuery searchQuery): base(userSession, searchQuery)
        {
            DefaultRedirectActionName = "Index";
            DefaultRedirectControllerName = "Home";
        }

    }
}
