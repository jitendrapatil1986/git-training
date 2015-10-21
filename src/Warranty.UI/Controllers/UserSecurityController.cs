using Common.Security.Queries;
using Common.UI.Security.Session;

namespace Warranty.UI.Controllers
{
    public class UserSecurityController : Common.UI.Security.Controllers.SecurityController
    {
        public UserSecurityController(IWebUserSession userSession, UserSearchQuery searchQuery): base(userSession, searchQuery)
        {
            DefaultRedirectActionName = "Index";
            DefaultRedirectControllerName = "Home";
        }

    }
}
