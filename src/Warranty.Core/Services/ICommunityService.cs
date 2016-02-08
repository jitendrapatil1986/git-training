using Warranty.Core.Entities;

namespace Warranty.Core.Services
{
    public interface ICommunityService
    {
        Community GetCommunityByNumber(string communityNumber);
    }
}