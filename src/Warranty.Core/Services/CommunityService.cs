using System;
using NPoco;
using Warranty.Core.Entities;

namespace Warranty.Core.Services
{
    public class CommunityService : ICommunityService
    {
        private IDatabase _database;

        public CommunityService(IDatabase database)
        {
            _database = database;
        }

        public string GetWarrantyCommunityNumber(string communityNumber)
        {
            if (communityNumber.Length > 4)
                return communityNumber.Substring(0, 4);

            if (communityNumber.Length < 4)
                return communityNumber.PadRight(4, '0');

            return communityNumber;
        }

        public Community Create(Community newCommunity)
        {
            if(newCommunity == null)
                throw new ArgumentNullException("newCommunity");

            _database.Insert(newCommunity);
            return newCommunity;
        }

        public City GetCity(string marketCode)
        {
            if (string.IsNullOrWhiteSpace(marketCode))
                return null;

            return _database.SingleOrDefault<City>("WHERE CityCode=@0", marketCode);
        }

        public City CreateCity(City city)
        {
            if (city == null)
                throw new ArgumentNullException("city");

            _database.Insert(city);
            return city;
        }

        public Division GetDivision(string divisionCode)
        {
            if (string.IsNullOrWhiteSpace(divisionCode))
                return null;

            return _database.SingleOrDefault<Division>("WHERE DivisionCode=@0", divisionCode);
        }

        public Division CreateDivision(Division division)
        {
            if (division == null)
                throw new ArgumentNullException("division");

            _database.Insert(division);
            return division;
        }

        public Project GetProject(string projectCode)
        {
            if (string.IsNullOrWhiteSpace(projectCode))
                return null;

            return _database.SingleOrDefault<Project>("WHERE ProjectNumber=@0", projectCode);
        }

        public Project CreateProject(Project project)
        {
            if(project == null)
                throw new ArgumentNullException("project");

            _database.Insert(project);
            return project;
        }

        public Community GetCommunityByNumber(string communityNumber)
        {
            if (string.IsNullOrWhiteSpace(communityNumber))
                throw new ArgumentNullException("communityNumber");

            return _database.SingleOrDefault<Community>("WHERE CommunityNumber = @0", GetWarrantyCommunityNumber(communityNumber));
        }
    }
}