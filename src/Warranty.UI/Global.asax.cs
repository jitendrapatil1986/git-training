using Warranty.UI.Core.Initialization;

namespace Warranty.UI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            WarrantyWebsiteBootstrapper.Boostrap();

#if DEBUG
            AutoMapper.Mapper.AssertConfigurationIsValid();
#endif
        }
    }
}