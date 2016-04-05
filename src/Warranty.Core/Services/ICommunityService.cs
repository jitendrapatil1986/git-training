using Warranty.Core.Entities;

namespace Warranty.Core.Services
{
    public interface ICommunityService
    {
        Community GetCommunityByNumber(string communityNumber);
        string GetWarrantyCommunityNumber(string communityNumber);
        Community Create(Community newCommunity);
        City GetCity(string marketCode);
        City CreateCity(City city);
        Division GetDivision(string divisionCode);
        Division CreateDivision(Division division);
        Project GetProject(string projectCode);
        Project CreateProject(Project project);
    }
}