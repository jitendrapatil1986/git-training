using System;
using StructureMap;
using Warranty.Core;

namespace Warranty.UI.Core.Initialization
{
    public static class IoC
    {
        private static readonly Lazy<IContainer> _container = new Lazy<IContainer>(Initialize);

        public static IContainer Container
        {
            get { return _container.Value; }
        }

        private static IContainer Initialize()
        {
            var container = new Container(cfg =>
                cfg.Scan(scan =>
                {
                    scan.AssemblyContainingType<IMediator>();
                    scan.AssemblyContainingType<WarrantyWebsiteRegistry>();

                    scan.AddAllTypesOf((typeof(IQueryHandler<,>)));
                    scan.AddAllTypesOf((typeof(ICommandHandler<>)));
                    scan.AddAllTypesOf((typeof(ICommandHandler<,>)));
                    scan.AddAllTypesOf((typeof(ICommandResultHandler<,>)));

                    scan.WithDefaultConventions();
                    scan.LookForRegistries();
                }));

            return container;
        }
    }
}