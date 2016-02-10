using TIPS.Events.Models;
using Job = Warranty.Core.Entities.Job;

namespace Warranty.Core.Services
{
    public interface IJobService
    {
        Job GetJobByNumber(string jobNumber);

        Job CreateJobAndInsert(TIPS.Events.Models.Job tipsJob);

        Job  CreateJobAndInsert(Sale sale);

        void UpdateExistingJob(Job job, Sale sale);

        Job CreateJobFromSale(Sale sale);

        Job CreateJobFromTipsJob(TIPS.Events.Models.Job tipsJob);

        void UpdateExistingJob(Job job, TIPS.Events.Models.Job tipsJob);
    }
}