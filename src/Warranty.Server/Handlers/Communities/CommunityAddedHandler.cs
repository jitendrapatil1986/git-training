﻿namespace Warranty.Server.Handlers.Communities
{
    using Accounting.Events.Community;
    using Core.Entities;
    using NPoco;
    using NServiceBus;

    public class CommunityAddedHandler : IHandleMessages<CommunityAdded>
    {
        private readonly IDatabase _database;

        public CommunityAddedHandler(IDatabase database)
        {
            _database = database;
        }

        public void Handle(CommunityAdded message)
        {
            using (_database)
            {
                var communityNumber = message.Community.JDEId.Substring(0, 4);
                var community = _database.SingleOrDefault<Community>("WHERE CommunityNumber=@0", communityNumber);
                if (community != null)
                    return;

                var city = _database.SingleOrDefault<City>("WHERE CityCode=@0", message.Market.JDEId);
                if (city == null)
                {
                    city = new City
                               {
                                   CityCode = message.Market.JDEId,
                                   CityName = message.Market.Name,
                               };
                    _database.Insert(city);
                }

                var division = _database.SingleOrDefault<Division>("WHERE DivisionCode=@0", message.Division.JDEId);
                if (division == null)
                {
                    division = new Division
                                   {
                                       DivisionCode = message.Division.JDEId,
                                       DivisionName = message.Division.Name,
                                       AreaCode = message.Area.JDEId,
                                       AreaName = message.Area.Name,
                                   };
                    _database.Insert(division);
                }

                var project = _database.SingleOrDefault<Project>("WHERE ProjectNumber=@0", message.Project.JDEId);
                if (project == null)
                {
                    project = new Project
                                  {
                                      ProjectNumber = message.Project.JDEId,
                                      ProjectName = message.Project.Name,
                                  };
                    _database.Insert(project);
                }

                community = new Community
                {
                    CityId = city.CityId,
                    CommunityName = message.Community.Name,
                    CommunityNumber = communityNumber,
                    CommunityStatusCode = message.Status.JDEId,
                    CommunityStatusDescription = message.Status.Name,
                    DivisionId = division.DivisionId,
                    ProductType = message.ProductType.Name,
                    ProductTypeDescription = message.ProductType.JDEId,
                    SateliteCityId = city.CityId,
                    ProjectId = project.ProjectId,
                };

                _database.Insert(community);
            }
        }
    }
}
