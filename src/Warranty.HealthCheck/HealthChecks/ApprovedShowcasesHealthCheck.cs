using NServiceBus;
using Quartz;
using Warranty.HealthCheck.Handlers;

namespace Warranty.HealthCheck.HealthChecks
{
    public class ApprovedShowcasesHealthCheck : IHealthCheck
    {
        private readonly IBus _bus;

        public ApprovedShowcasesHealthCheck(IBus bus)
        {
            _bus = bus;
        }

        public void Execute(IJobExecutionContext context)
        {
            _bus.SendLocal(new InitiateApprovedShowcasesHealthCheckSaga());
        }

        public void Schedule(IScheduler scheduler)
        {
            var schedule = TriggerBuilder.Create()
                .WithIdentity("ApprovedShowcasesTrigger")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(5, 0))
                .Build();

            var detail = JobBuilder.Create<SoldJobsHealthCheck>()
                .WithIdentity("ApprovedShowcasesCheck")
                .Build();

            scheduler.ScheduleJob(detail, schedule);
        }
    }
}