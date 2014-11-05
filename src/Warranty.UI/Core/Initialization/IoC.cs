using FluentValidation;
using Warranty.Core.ActivityLogger;
using Warranty.Core.ApprovalInfrastructure.Interfaces;

namespace Warranty.UI.Core.Initialization
{
    using System;
    using StructureMap;
    using Warranty.Core;
    using Warranty.Core.Calculator;
    using Warranty.Core.CsvBuilder;
    using Warranty.Core.TaskInfrastructure.Interfaces;

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

                    scan.AddAllTypesOf(typeof(IValidator<>));
                    scan.AddAllTypesOf((typeof(IQueryHandler<,>)));
                    scan.AddAllTypesOf((typeof(ICommandHandler<>)));
                    scan.AddAllTypesOf((typeof(ICommandHandler<,>)));
                    scan.AddAllTypesOf((typeof(ICommandResultHandler<,>)));
                    scan.AddAllTypesOf((typeof(IApprovalService<>)));
                    scan.AddAllTypesOf((typeof(IActivityLogger)));
                    scan.AddAllTypesOf((typeof(ICsvBuilder)));
                    scan.AddAllTypesOf((typeof(ITask<>)));
                    scan.AddAllTypesOf((typeof(IWarrantyCalculator)));

                    scan.WithDefaultConventions();
                });
            });

            return container;
        }
    }
}
