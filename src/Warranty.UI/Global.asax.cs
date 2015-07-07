using System;

namespace Warranty.UI
{
    using Core.Initialization;
    using Microsoft.Practices.ServiceLocation;
    using Warranty.Core;
    using Warranty.Core.Features.SessionStart;
    using System.IdentityModel.Services;
    using SystemTokens = System.IdentityModel.Tokens;

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Session_Start()
        {
            var mediator = ServiceLocator.Current.GetInstance<IMediator>();
            mediator.Send(new VerifyEmployeeExistsCommand());
        }

        protected void Application_Start()
        {
            WarrantyWebsiteBootstrapper.Boostrap();

#if DEBUG
            AutoMapper.Mapper.AssertConfigurationIsValid();
#endif
        }

        void SessionAuthenticationModule_SessionSecurityTokenReceived(object sender, SessionSecurityTokenReceivedEventArgs e)
        {
            var now = DateTime.UtcNow;
            var sst = e.SessionToken;
            var validTo = sst.ValidTo;
            var validFrom = sst.ValidFrom;
            const int sessionLifeInMinutes = 60;
            const int reissueCookieWhenMinutesRemain = 30;

            if ((validTo - validFrom).TotalMinutes > sessionLifeInMinutes
                || ((now < validTo) && (now > validTo.AddMinutes(-(sessionLifeInMinutes - reissueCookieWhenMinutesRemain)))))
            {
                var sam = sender as SessionAuthenticationModule;
                if (sam == null) 
                    return;

                e.SessionToken = sam.CreateSessionSecurityToken(sst.ClaimsPrincipal,
                    sst.Context,
                    now,
                    now.AddMinutes(sessionLifeInMinutes),
                    sst.IsPersistent);
                e.ReissueCookie = true;
            }
        }
    }
}