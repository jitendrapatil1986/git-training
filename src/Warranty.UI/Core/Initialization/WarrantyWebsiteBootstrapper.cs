namespace Warranty.UI.Core.Initialization
{
    using System.Web.Http;
    using System.Web.Mvc;
    using FluentValidation.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using ModelBinders;
    using NServiceBus;
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
            FluentValidationModelValidatorProvider.Configure();

            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;

            ModelBinders.Binders.DefaultBinder = new CompositeModelBinder();
            ModelMetadataProviders.Current = new ModelMetadataProvider();


            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            DbFactory.Setup(IoC.Container);
            InitializeNServiceBus();
        }

        private static void InitializeNServiceBus()
        {
           Configure.With()
                     .StructureMapBuilder(IoC.Container)
                     .UseTransport<Msmq>()
                     .UnicastBus()
                     .RunHandlersUnderIncomingPrincipal(false)
                     .MsmqSubscriptionStorage()
                     .SendOnly();
        }
    }
}