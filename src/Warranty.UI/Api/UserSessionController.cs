using System;
using System.IdentityModel.Services;
using System.Web.Http;

namespace Warranty.UI.Api
{
    public class UserSessionController : ApiController
    {
        [HttpGet]
        public DateTime KeepAlive()
        {
            var sessionModule = FederatedAuthentication.SessionAuthenticationModule;
            return sessionModule.ContextSessionSecurityToken.ValidTo;
        }
    }
}