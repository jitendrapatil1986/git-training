using System;
using Quartz;
using Quartz.Spi;
using StructureMap;

namespace Warranty.HealthCheck.Config
{
    public class StructuremapJobFactory : IJobFactory
    {
        private readonly IContainer _container;

        public StructuremapJobFactory(IContainer container)
        {
            _container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return (IJob)_container.GetInstance(bundle.JobDetail.JobType);
        }

        public void ReturnJob(IJob job)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            var disposable = job as IDisposable;
            if (disposable == null)
                return;

            disposable.Dispose();
        }
    }
}