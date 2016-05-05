using System.Collections.Generic;
using log4net;
using NServiceBus;
using Quartz;
using Warranty.HealthCheck.HealthChecks;

namespace Warranty.HealthCheck.Startup
{
    public class SchedulerStartup : IWantToRunWhenBusStartsAndStops
    {
        private readonly ILog _log;
        public IScheduler Scheduler { get; set; }

        public SchedulerStartup(ILog log, IScheduler scheduler, IEnumerable<IHealthCheck> jobs)
        {
            Scheduler = scheduler;
            _log = log;

            foreach (var healthCheck in jobs)
            {
                healthCheck.Schedule(Scheduler);
            }
        }

        public void Start()
        {
            _log.Info("Scheduler is starting");
            Scheduler.Start();
        }

        public void Stop()
        {
            _log.Info("Scheduler is shutting down");
            Scheduler.Shutdown();
        }
    }
}