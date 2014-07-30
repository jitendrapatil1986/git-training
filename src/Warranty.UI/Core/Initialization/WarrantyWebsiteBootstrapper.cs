using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using AutoMapper;
using NServiceBus;
using Warranty.Core.AutoMapper;
using Warranty.UI.Core.ModelBinders;

namespace Warranty.UI.Core.Initialization
{
    public class WarrantyWebsiteBootstrapper
    {
        public static void Boostrap()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;

            System.Web.Mvc.ModelBinders.Binders.DefaultBinder = new CompositeModelBinder();
            ModelMetadataProviders.Current = new ModelMetadataProvider();


            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

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