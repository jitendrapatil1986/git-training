using System;
using System.Collections.Generic;
using System.Linq;
using Common.Security.Session;
using NPoco;
using Warranty.Core.Entities;

namespace Warranty.Core.Features.MyProjects
{
    public class MyProjectsQueryHandler : IQueryHandler<MyProjectsQuery, Dictionary<Guid, string>>
    {
        private readonly IUserSession _userSession;
        private readonly IDatabase _database;

        public MyProjectsQueryHandler(IUserSession userSession, IDatabase database)
        {
            _userSession = userSession;
            _database = database;
        }

        public Dictionary<Guid, string> Handle(MyProjectsQuery query)
        {
            var user = _userSession.GetCurrentUser();
            var markets = user.Markets;

            var sql = @"SELECT DISTINCT
	                        p.ProjectId,
	                        p.ProjectName
                        FROM 
	                        Communities com
	                        LEFT JOIN Cities c ON c.CityId = com.CityId
	                        LEFT JOIN Divisions d on d.DivisionId = com.DivisionId
	                        LEFT JOIN Projects p on p.ProjectId = com.ProjectId
                        WHERE d.DivisionId IS NOT NULL AND p.ProjectId IS NOT NULL ";

            if (query.DivisionId.HasValue)
                sql += "AND d.DivisionId = @0";

            var result = _database.Fetch<Project>(sql, query.DivisionId);
            var projects = new Dictionary<Guid, string>();

            if (result == null || !result.Any())
                return projects;

            foreach (var project in result)
            {
                projects.Add(project.ProjectId, project.ProjectName);
            }

            return projects;
        }
        
    }
}