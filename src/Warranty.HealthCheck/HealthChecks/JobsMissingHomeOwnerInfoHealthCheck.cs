using log4net;
using NServiceBus;
using Quartz;

namespace Warranty.HealthCheck.HealthChecks
{
    public class JobsMissingHomeOwnerInfoHealthCheck : IHealthCheck
    {
        private readonly IBus _bus;
        private readonly ILog _log;

        public JobsMissingHomeOwnerInfoHealthCheck(IBus bus, ILog log)
        {
            _bus = bus;
            _log = log;
        }

        public void Execute(IJobExecutionContext context)
        {
            throw new System.NotImplementedException("Must implement the HealthCheck Execute() function.");
        }

        public void Schedule(IScheduler scheduler)
        {
            var schedule = TriggerBuilder.Create()
                .WithIdentity("JobsMissingHomeOwnerInfoTrigger")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(5, 0))
                .Build();

            var detail = JobBuilder.Create<SoldJobsHealthCheck>()
                .WithIdentity("JobsMissingHomeOwnerInfoJobsCheck")
                .Build();

            scheduler.ScheduleJob(detail, schedule);
        }
    }
}