using System;
using System.IdentityModel.Services;
using System.Web.Http;

namespace Warranty.UI.Api
{
    public class UserSessionController : ApiController
    {
        [HttpGet]
        public double KeepAlive()
        {
            var sessionModule = FederatedAuthentication.SessionAuthenticationModule;
            var now = DateTime.UtcNow;
            return (sessionModule.ContextSessionSecurityToken.ValidTo - now).TotalMilliseconds;
        }
    }
}