using Quartz;

namespace Warranty.HealthCheck.HealthChecks
{
    public interface IHealthCheck : IJob
    {
        void Schedule(IScheduler scheduler);
    }
}