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
            var sessionModule = FederatedAuthentication.SessionAuthenticationModule;
            return (sessionModule != null ? 
                sessionModule.ContextSessionSecurityToken.ValidTo.ToString() 
                : DateTime.UtcNow.AddMinutes(5).ToString()) + " UTC";
        }

        [HttpGet]
        public string SignOut()
        {
            _userSession.LogOut();
            return "success";
        }
    }
}