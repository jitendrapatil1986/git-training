using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using Warranty.Core.Security;

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