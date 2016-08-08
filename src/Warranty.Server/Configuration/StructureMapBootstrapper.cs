using log4net;
using StructureMap;
using Warranty.Core;

namespace Warranty.Server.Configuration
{
    internal static class StructureMapBootstrapper
    {
        public static void Bootstrap(Container container)
        {
            container.Configure(cfg =>
            {
                cfg.Scan(x =>
                {
                    x.AssemblyContainingType<IMediator>();
                    x.AddAllTypesOf((typeof(IQueryHandler<,>)));
                    x.AddAllTypesOf((typeof(ICommandHandler<>)));
                    x.AddAllTypesOf((typeof(ICommandHandler<,>)));
                    x.WithDefaultConventions();
                });
                cfg.For<ILog>().AlwaysUnique().Use(c =>
                {
                    var parentType = c.ParentType ?? c.BuildStack.Current.ConcreteType;
                    return LogManager.GetLogger(parentType);
                });

            });
        }
    }
}