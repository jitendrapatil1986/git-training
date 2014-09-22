using FluentValidation;
using Warranty.Core.ActivityLogger;
using Warranty.Core.ApprovalInfrastructure.Interfaces;

namespace Warranty.Core
{
    using DataAccess;
    using Entities;
    using NPoco;
    using NPoco.FluentMappings;
    using StructureMap.Configuration.DSL;

    public class WarrantyCoreRegistry : Registry
    {
        public WarrantyCoreRegistry()
        {
            Scan(x =>
                     {
                         x.AssemblyContainingType<IAuditableEntity>();
                         x.AddAllTypesOf<IMap>();
                         x.AssemblyContainingType<IMediator>();
                         x.AddAllTypesOf(typeof(IValidator<>));
                         x.AddAllTypesOf((typeof(IQueryHandler<,>)));
                         x.AddAllTypesOf((typeof(ICommandHandler<>)));
                         x.AddAllTypesOf((typeof(ICommandHandler<,>)));
                         x.AddAllTypesOf((typeof(ICommandResultHandler<,>)));
                         x.AddAllTypesOf((typeof(IApprovalService<>)));
                         x.AddAllTypesOf((typeof(IActivityLogger)));
                         x.WithDefaultConventions();
                     });

            For<IDatabase>().Use(() => DbFactory.DatabaseFactory.GetDatabase());
        }
    }
}
