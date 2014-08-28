using Warranty.Core.ApprovalInfrastructure.Interfaces;

namespace Warranty.UI.Core.Initialization
{
    using System;
    using StructureMap;
    using Warranty.Core;

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
            {
                cfg.AddRegistry<WarrantyCoreRegistry>();
                cfg.AddRegistry<WarrantyWebsiteRegistry>();
                cfg.Scan(scan =>
                {
                    scan.AssemblyContainingType<IMediator>();

                    scan.AddAllTypesOf((typeof(IQueryHandler<,>)));
                    scan.AddAllTypesOf((typeof(ICommandHandler<>)));
                    scan.AddAllTypesOf((typeof(ICommandHandler<,>)));
                    scan.AddAllTypesOf((typeof(ICommandResultHandler<,>)));
                    scan.AddAllTypesOf((typeof(IApprovalService<>)));

                    scan.WithDefaultConventions();
                });
            });

            return container;
        }
    }
}