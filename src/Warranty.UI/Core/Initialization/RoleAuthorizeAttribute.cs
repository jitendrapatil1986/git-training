using System.Web;
using System.Web.Mvc;
using Common.Security.Session;
using Microsoft.Practices.ServiceLocation;

namespace Warranty.UI.Core.Initialization
{
    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string _allowedRole;

        public RoleAuthorizeAttribute(string allowedRole)
        {
            _allowedRole = allowedRole;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var userSession = ServiceLocator.Current.GetInstance<IUserSession>();
            var user = userSession.GetCurrentUser();

            return user.IsInRole(_allowedRole);
        }
    }
}