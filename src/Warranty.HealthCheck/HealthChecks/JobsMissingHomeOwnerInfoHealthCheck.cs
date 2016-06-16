using System;
using log4net;
using NServiceBus;
using Quartz;
using Warranty.HealthCheck.Handlers;

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
            _bus.SendLocal(new InitiateJobsMissingHomeOwnerInfoHealthCheckSaga(DateTime.Parse("01/01/2016")));
        }

        public void Schedule(IScheduler scheduler)
        {
            var schedule = TriggerBuilder.Create()
                .WithIdentity("JobsMissingHomeOwnerInfoTrigger")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(5, 10))
                .Build();

            var detail = JobBuilder.Create<JobsMissingHomeOwnerInfoHealthCheck>()
                .WithIdentity("JobsMissingHomeOwnerInfoJobsCheck")
                .Build();

            scheduler.ScheduleJob(detail, schedule);
        }
    }
}