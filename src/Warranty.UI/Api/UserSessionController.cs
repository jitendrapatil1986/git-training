using System;
using System.Web.Http;
using Microsoft.IdentityModel.Web;
using Warranty.Core.Security;

namespace Warranty.UI.Api
{
    public class UserSessionController : ApiController
    {

        public UserSessionController(IUserSession userSession)
        {
            _userSession = userSession;
        }

        private readonly IUserSession _userSession;

        [HttpGet]
        public string Ping()
        {
#if DEBUG
            return DateTime.UtcNow.AddMinutes(5) + " UTC";
#else
            var sessionModule = FederatedAuthentication.SessionAuthenticationModule;
            return sessionModule.ContextSessionSecurityToken.ValidTo + " UTC";
#endif

        }

        [HttpGet]
        public string SignOut()
        {
            _userSession.LogOut();
            return "success";
        }
    }
}