namespace Warranty.UI
{
    using Core.Initialization;

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