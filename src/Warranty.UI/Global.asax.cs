namespace Warranty.UI
{
    using Core.Initialization;
    using Microsoft.Practices.ServiceLocation;
    using Warranty.Core;
    using Warranty.Core.Features.SessionStart;

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
    }
}