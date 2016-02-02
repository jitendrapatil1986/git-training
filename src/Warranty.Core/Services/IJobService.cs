using System;
using TIPS.Events.Models;
using Job = Warranty.Core.Entities.Job;

namespace Warranty.Server.Handlers.Jobs
{
    public interface IJobService
    {
        Job GetJobByNumber(string jobNumber);

        Job GetJob(Sale sale);

        Job GetJobAndInsert(Sale sale);
    }
}