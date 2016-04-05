using System;
using TIPS.Events.Models;
using Job = Warranty.Core.Entities.Job;

namespace Warranty.Core.Services
{
    public interface IJobService
    {
        Job GetJobByNumber(string jobNumber);

        Job CreateJob(TIPS.Events.Models.Job tipsJob);

        Job CreateJob(Sale sale);

        void UpdateExistingJob(Job job, Sale sale);

        void UpdateExistingJob(Job job, TIPS.Events.Models.Job tipsJob);

        Job GetJobById(Guid jobId);

        bool IsModelOrShowcase(Job job);

        void Save(Job job);

        Job CreateJob(Job job);
        void UpdateExistingJob(Job job);
    }
}