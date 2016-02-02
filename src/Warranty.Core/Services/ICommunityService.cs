using Warranty.Core.Entities;

namespace Warranty.Server.Handlers.Jobs
{
    public interface ICommunityService
    {
        Community GetCommunityByNumber(string communityNumber);
    }
}