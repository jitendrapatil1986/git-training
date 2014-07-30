using System.Web.Http;
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using StructureMap;
using Warranty.UI.Core.Initialization;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Warranty.UI.StructuremapMvc), "Start")]

namespace Warranty.UI
{
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