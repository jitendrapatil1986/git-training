using System.Collections.Generic;
using System.Linq;
using log4net;
using MediatR;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;
using Warranty.HealthCheck.Data;
using Warranty.HealthCheck.HealthChecks;

namespace Warranty.HealthCheck.Config
{
    public class HealthCheckRegistry : Registry
    {
        public HealthCheckRegistry()
        {
            Scan(scanner =>
            {
                scanner.WithDefaultConventions();
                scanner.TheCallingAssembly();
                scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                scanner.ConnectImplementationsToTypesClosing(typeof(IAsyncRequestHandler<,>));
                scanner.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                scanner.ConnectImplementationsToTypesClosing(typeof(IAsyncNotificationHandler<>));
                scanner.AddAllTypesOf<IHealthCheck>();

                For<SingleInstanceFactory>().Use(ctx => t => ctx.GetInstance(t));
                For<MultiInstanceFactory>().Use(ctx => t => ctx.GetAllInstances(t));
                For<IMediator>().Use<Mediator>();

                For<ILog>().AlwaysUnique().Use(c =>
                {
                    var parentType = c.ParentType ?? c.BuildStack.Current.ConcreteType;
                    return LogManager.GetLogger(parentType);
                });

                For<ITipsDatabase>()
                    .LifecycleIs(new ThreadLocalStorageLifecycle())
                    .Use(new TipsDatabase("tips"));

                For<IWarrantyDatabase>()
                    .LifecycleIs(new ThreadLocalStorageLifecycle())
                    .Use(new WarrantyDatabase("warranty"));

                For<IHealthCheckDatabase>()
                    .LifecycleIs(new ThreadLocalStorageLifecycle())
                    .Use(new HealthCheckDatabase("NServiceBus/Persistence"));

                For<IJobFactory>().Use<StructuremapJobFactory>();
                For<ISchedulerFactory>().Use(new StdSchedulerFactory());
                For<IScheduler>()
                    .LifecycleIs(new SingletonLifecycle())
                    .Use(ctx =>
                    {
                        var scheduler = ctx.GetInstance<ISchedulerFactory>().GetScheduler();
                        scheduler.JobFactory = ctx.GetInstance<IJobFactory>();
                        return scheduler;
                    });

            });
        }
    }
}