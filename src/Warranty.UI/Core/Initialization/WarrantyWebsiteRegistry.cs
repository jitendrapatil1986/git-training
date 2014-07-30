using System.Web;
using System.Web.Mvc;
using FluentValidation;
using FluentValidation.Mvc;
using StructureMap.Configuration.DSL;
using Warranty.Core;
using Warranty.Core.Security;
using Warranty.UI.Core.Security;

namespace Warranty.UI.Core.Initialization
{
    public class WarrantyWebsiteRegistry : Registry
    {
        public WarrantyWebsiteRegistry()
        {
            For<IValidatorFactory>().Use<StructureMapValidatorFactory>();
            For<ModelValidatorProvider>().Use<FluentValidationModelValidatorProvider>();
            For<HttpRequest>().Use(() => HttpContext.Current.Request);
            For<IUserSession>().Use<WarrantyUserSession>();
        }
    }
}