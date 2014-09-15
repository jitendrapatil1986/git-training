using Warranty.UI.Mailer;

namespace Warranty.UI.Core.Initialization
{
    using System.Web;
    using System.Web.Mvc;
    using FluentValidation;
    using FluentValidation.Mvc;
    using Security;
    using StructureMap.Configuration.DSL;
    using Warranty.Core;
    using Warranty.Core.Security;

    public class WarrantyWebsiteRegistry : Registry
    {
        public WarrantyWebsiteRegistry()
        {
            For<IValidatorFactory>().Use<StructureMapValidatorFactory>();
            For<ModelValidatorProvider>().Use<FluentValidationModelValidatorProvider>();
            For<HttpRequest>().Use(() => HttpContext.Current.Request);
            For<IUserSession>().Use<WarrantyUserSession>();
            For<IWarrantyMailer>().Use<WarrantyMailer>();
 }
    }
}