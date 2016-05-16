﻿using System;
using NServiceBus;
using Quartz;
using Warranty.HealthCheck.Handlers;

namespace Warranty.HealthCheck.HealthChecks
{
    public class SoldJobsHealthCheck : IHealthCheck
    {
        private readonly IBus _bus;

        public SoldJobsHealthCheck(IBus bus)
        {
            _bus = bus;
        }

        public void Execute(IJobExecutionContext context)
        {
            _bus.SendLocal(new InitiateSoldJobsHealthCheckSaga(new DateTime(2016, 1, 1)));
        }

        public void Schedule(IScheduler scheduler)
        {
            var schedule = TriggerBuilder.Create()
                .WithIdentity("SoldJobsTrigger")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(5, 0))
                .Build();

            var detail = JobBuilder.Create<SoldJobsHealthCheck>()
                .WithIdentity("SoldJobsCheck")
                .Build();

            scheduler.ScheduleJob(detail, schedule);
        }
    }
}