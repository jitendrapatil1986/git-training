using System.Collections.Generic;
using TIPS.Events.Models;
using Warranty.Core.Entities;
using Job = Warranty.Core.Entities.Job;

namespace Warranty.Server.Handlers.Jobs
{
    public interface IHomeOwnerService
    {
        void AssignToJob(HomeOwner homeOwner, Job job);

        HomeOwner GetHomeOwnerByJobNumber(string jobNumber);

        HomeOwner GetHomeOwner(Contact homeOwnerInfo, int homeOwnerNumber = 1);

        void RemoveFromJob(HomeOwner homeOwner, Job job);
        HomeOwner Create(HomeOwner homeOwner);
    }
}