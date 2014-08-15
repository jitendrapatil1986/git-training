using Warranty.UI.Core.Security;

namespace Warranty.UI.Core.Initialization
{
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using AutoMapper;
    using ModelBinders;
    using NServiceBus;
    using Warranty.Core.AutoMapper;
    using Warranty.Core.DataAccess;

    public class WarrantyWebsiteBootstrapper
    {
        public static void Boostrap()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;

            System.Web.Mvc.ModelBinders.Binders.DefaultBinder = new CompositeModelBinder();
            ModelMetadataProviders.Current = new ModelMetadataProvider();


            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            DbFactory.Setup(new WarrantyUserSession());
            InitializeAutoMapper();
            InitializeNServiceBus();
        }

        private static void InitializeNServiceBus()
        {
            Configure.With()
                .StructureMapBuilder()
                .UseTransport<Msmq>()
                .UnicastBus()
                .RunHandlersUnderIncomingPrincipal(false)
                .MsmqSubscriptionStorage()
                .CreateBus()
                .Start(() =>
                    Configure.Instance.ForInstallationOn<NServiceBus.Installation.Environments.Windows>().Install()
                    );
        }

        private static void InitializeAutoMapper()
        {
            Mapper.Initialize(cfg => cfg.AddProfile<QueryAutoMapperProfile>());
        }
    }
}