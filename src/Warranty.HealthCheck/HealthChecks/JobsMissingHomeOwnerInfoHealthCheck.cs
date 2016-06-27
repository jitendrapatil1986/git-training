using System;
using NServiceBus;
using Quartz;
using Warranty.HealthCheck.Handlers;

namespace Warranty.HealthCheck.HealthChecks
{
    public class JobsMissingHomeOwnerInfoHealthCheck : IHealthCheck
    {
        private readonly IBus _bus;

        public JobsMissingHomeOwnerInfoHealthCheck(IBus bus)
        {
            _bus = bus;
        }

        public void Execute(IJobExecutionContext context)
        {
            _bus.SendLocal(new InitiateJobsMissingHomeOwnerInfoHealthCheckSaga(new DateTime(2016, 1, 1)));
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