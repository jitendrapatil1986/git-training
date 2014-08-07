[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Warranty.UI.StructuremapMvc), "Start")]

namespace Warranty.UI
{
    using System.Web.Http;
    using System.Web.Mvc;
    using Core.Initialization;
    using Microsoft.Practices.ServiceLocation;
    using StructureMap;

    public class StructuremapMvc
    {
        public static void Start()
        {
            IContainer container = IoC.Container;
            ServiceLocator.SetLocatorProvider(() => new StructureMapDependencyScope(container));
            DependencyResolver.SetResolver(new StructureMapDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new StructureMapDependencyResolver(container);
        }
    }
}