using log4net;
using StructureMap;

namespace Warranty.Server.Configuration
{
    internal static class StructureMapBootstrapper
    {
        public static void Bootstrap(Container container)
        {
            container.Configure(cfg =>
            {
                cfg.For<ILog>().AlwaysUnique().Use(c =>
                {
                    var parentType = c.ParentType ?? c.BuildStack.Current.ConcreteType;
                    return LogManager.GetLogger(parentType);
                });
            });
        }
    }
}