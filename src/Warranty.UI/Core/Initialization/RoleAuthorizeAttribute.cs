using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common.Security.Session;
using Microsoft.Practices.ServiceLocation;

namespace Warranty.UI.Core.Initialization
{
    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] _allowedRoles;

        public RoleAuthorizeAttribute(params string[] allowedRoleses)
        {
            _allowedRoles = allowedRoleses;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var userSession = ServiceLocator.Current.GetInstance<IUserSession>();
            var user = userSession.GetCurrentUser();

            return _allowedRoles.Any(r => user.IsInRole(r));
        }
    }
}