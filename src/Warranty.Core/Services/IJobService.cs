using TIPS.Events.Models;
using Job = Warranty.Core.Entities.Job;

namespace Warranty.Core.Services
{
    public interface IJobService
    {
        Job GetJobByNumber(string jobNumber);

        Job CreateJobFromSale(Sale sale);

        Job CreateJobAndInsert(Sale sale);

        void UpdateExistingJob(Job job, Sale sale);
    }
}