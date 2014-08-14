using Warranty.Core.Security;

namespace Warranty.UI
{
    using Core.Initialization;

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start(IUserSession userSession)
        {
            WarrantyWebsiteBootstrapper.Boostrap(userSession);

#if DEBUG
            AutoMapper.Mapper.AssertConfigurationIsValid();
#endif
        }
    }
}