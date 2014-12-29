using FluentValidation;
using Warranty.Core.ActivityLogger;
using Warranty.Core.ApprovalInfrastructure.Interfaces;

namespace Warranty.UI.Core.Initialization
{
    using System;
    using Accounting.Client;
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
                    scan.AssemblyContainingType<IAccountingClient>();
                    scan.AddAllTypesOf((typeof(ICsvBuilder)));
                    scan.AddAllTypesOf((typeof(ITask<>)));
                    scan.AddAllTypesOf((typeof(IWarrantyCalculator)));
                    scan.AddAllTypesOf((typeof(IAccountingClient)));

                    scan.WithDefaultConventions();
                });

            });

            return container;
        }
    }
}
