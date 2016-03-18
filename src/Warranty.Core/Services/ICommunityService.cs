using Warranty.Core.Entities;

namespace Warranty.Core.Services
{
    public interface ICommunityService
    {
        Community GetCommunityByNumber(string communityNumber);

        string GetWarrantyCommunityNumber(string communityNumber);
        Community Create(Community newCommunity);
    }
}